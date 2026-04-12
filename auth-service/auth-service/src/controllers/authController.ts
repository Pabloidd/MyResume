// src/controllers/authController.ts

import type { Request, Response } from 'express';
import bcrypt from 'bcryptjs';
import { User } from '../models/User.js';
import { generateCode, generateToken } from '../utils/token.js';
import { createFingerprint } from '../utils/fingerprint.js';
import { sendVerificationCode, sendResetCode } from '../utils/email.js';
import type { AuthRequest } from '../types/index.js';
import { 
  AuthenticationError, 
  AuthorizationError, 
  NotFoundError, 
  ConflictError, 
  EmailError,
  ValidationError
} from '../errors/AppError.js';
import { ErrorCodes } from '../errors/errorCodes.js';
import { catchAsync } from '../errors/errorHandler.js';

// 1. Рег.
export const register = catchAsync(async (req: Request, res: Response) => {
  const { email, password } = req.body;
  
  // ЕСЛИ email уже существует
  const existingUser = await User.findOne({ email });
  if (existingUser) {
    throw new ConflictError('Email already exists', ErrorCodes.EMAIL_ALREADY_EXISTS);
  }
  
  // ЕСЛИ всё хорошо — создаём пользователя
  const passwordHash = await bcrypt.hash(password, 10);
  const verificationCode = generateCode();
  const codeExpiresAt = new Date(Date.now() + 15 * 60 * 1000);

  const user = new User({
    email,
    passwordHash,
    status: 'pending',
    role: 'user',                    
    verificationCode,
    verificationCodeExpiresAt: codeExpiresAt,
    pendingExpiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000),
    verificationAttempts: 0
  });
  
  await user.save();
  
  // ЕСЛИ письмо не отправилось
  try {
    await sendVerificationCode(email, verificationCode);
  } catch (error) {
    throw new EmailError('Failed to send verification code', ErrorCodes.EMAIL_ERROR);
  }
  
  res.json({
    success: true,
    message: 'Verification code sent to email',
    email
  });
});


// 2. Подтв. EMAIL
export const verify = catchAsync(async (req: Request, res: Response) => {
  const { email, code } = req.body;
  
  // ЕСЛИ пользователь не найден
  const user = await User.findOne({ email });
  if (!user) {
    throw new NotFoundError('User', ErrorCodes.USER_NOT_FOUND);
  }
  
  // ЕСЛИ код не совпадает
  if (user.verificationCode !== code) {
    throw new ValidationError('Invalid verification code', ErrorCodes.INVALID_CODE);
  }
  
  // ЕСЛИ код истек
  if (new Date() > user.verificationCodeExpiresAt!) {
    throw new ValidationError('Verification code expired', ErrorCodes.CODE_EXPIRED);
  }
  
  // ЕСЛИ email уже подтвержден
  if (user.status === 'active') {
    throw new ValidationError('Email already verified', ErrorCodes.INVALID_CODE);
  }
  
  // Всё хорошо — активируем и выдаём токен
  const secretToken = generateToken();
  const fingerprint = createFingerprint(req);
  const deviceName = req.headers['user-agent'] || 'Unknown Device';

  user.status = 'active';
  user.verificationCode = null;
  user.verificationCodeExpiresAt = null;
  user.secretToken = secretToken;
  user.tokenExpiresAt = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);  // 7 дней
  user.trustedDevices = [{
    fingerprint,
    lastSeen: new Date(),
    deviceName
  }];

  user.pendingExpiresAt = null;
  user.verificationAttempts = 0;

  await user.save();

  res.cookie('authToken', secretToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 7 * 24 * 60 * 60 * 1000  // 7 дней
  });
  
  res.json({
    success: true,
    message: 'Email verified successfully',
    user: { email: user.email, status: user.status }
  });
});

// 3. Вход
export const login = catchAsync(async (req: Request, res: Response) => {
  const { email, password } = req.body;
  
  // ЕСЛИ пользователь не найден
  const user = await User.findOne({ email });
  if (!user) {
    throw new AuthenticationError('Invalid credentials', ErrorCodes.INVALID_CREDENTIALS);
  }
  
  // ЕСЛИ email не подтвержден
  if (user.status !== 'active') {
    throw new AuthorizationError('Email not verified', ErrorCodes.EMAIL_NOT_VERIFIED);
  }
  
  // ЕСЛИ пароль неверный
  const isValid = await bcrypt.compare(password, user.passwordHash);
  if (!isValid) {
    throw new AuthenticationError('Invalid credentials', ErrorCodes.INVALID_CREDENTIALS);
  }
  
  // Всё хорошо — обновляем токен и слепок
  const secretToken = generateToken();
  const fingerprint = createFingerprint(req);
  const deviceName = req.headers['user-agent'] || 'Unknown Device';

  // Проверяем, есть ли уже это устройство
  const existingDeviceIndex = user.trustedDevices.findIndex(
    d => d.fingerprint === fingerprint
  );

  if (existingDeviceIndex >= 0) {
    // Обновляем lastSeen для существующего устройства
    const device = user.trustedDevices[existingDeviceIndex];
    if (device) {
      device.lastSeen = new Date();
    }
  } else {
    // Добавляем новое устройство
    user.trustedDevices.push({
      fingerprint,
      lastSeen: new Date(),
      deviceName
    });
  }

  // Обновляем токен и срок действия
  user.secretToken = secretToken;
  user.tokenExpiresAt = new Date(Date.now() + 7 * 24 * 60 * 60 * 1000);  // 7 дней

  await user.save();

  res.cookie('authToken', secretToken, {
    httpOnly: true,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'lax',
    maxAge: 7 * 24 * 60 * 60 * 1000  // 7 дней
  });
  
  res.json({
    success: true,
    message: 'Login successful',
    user: { 
      email: user.email, 
      status: user.status,
      role: user.role 
    }
  });
});

// 4. Профиль 
export const profile = catchAsync(async (req: AuthRequest, res: Response) => {
  const user = await User.findById(req.user?.userId).select('-passwordHash');
  //  если Не найден
  if (!user) {
    throw new NotFoundError('User', ErrorCodes.USER_NOT_FOUND);
  }
  
  res.json({
    success: true,
    user: {
      email: user.email,
      status: user.status,
      role: user.role,           
      createdAt: user.createdAt
    }
  });
});

// 5. Забыли пароль
export const forgotPassword = catchAsync(async (req: Request, res: Response) => {
  const { email } = req.body;
  
  const user = await User.findOne({ email });
  
  // ЕСЛИ пользователь не найден — не говорим об этом (безопасность)
  if (!user) {
    res.json({ message: 'If email exists, reset code sent' });
    return;
  }
  
  const resetCode = generateCode();
  user.resetCode = resetCode;
  user.resetCodeExpiresAt = new Date(Date.now() + 15 * 60 * 1000);
  
  await user.save();
  
  try {
    await sendResetCode(email, resetCode);
  } catch (error) {
    throw new EmailError('Failed to send reset code', ErrorCodes.EMAIL_ERROR);
  }
  
  res.json({ message: 'If email exists, reset code sent' });
});

// 6. Сброс пароля
export const resetPassword = catchAsync(async (req: Request, res: Response) => {
  const { email, code, newPassword } = req.body;
  
  const user = await User.findOne({ email });
  
  // ЕСЛИ пользователь не найден
  if (!user) {
    throw new NotFoundError('User', ErrorCodes.USER_NOT_FOUND);
  }
  if (user.status !== 'active') {
    throw new AuthorizationError('Email not verified', ErrorCodes.EMAIL_NOT_VERIFIED);
  }
  // ЕСЛИ код не совпадает
  if (user.resetCode !== code) {
    throw new ValidationError('Invalid reset code', ErrorCodes.INVALID_CODE);
  }
  
  // ЕСЛИ код истек
  if (new Date() > user.resetCodeExpiresAt!) {
    throw new ValidationError('Reset code expired', ErrorCodes.CODE_EXPIRED);
  }
  
  // Обновляем пароль
  const passwordHash = await bcrypt.hash(newPassword, 10);

  user.passwordHash = passwordHash;
  user.resetCode = null;
  user.resetCodeExpiresAt = null;
  user.secretToken = null;
  user.tokenExpiresAt = null;
  user.trustedDevices = [];

  await user.save();

  res.clearCookie('authToken');
  res.json({ success: true, message: 'Password reset successfully' });
});

// 7. Выход
export const logout = catchAsync(async (req: AuthRequest, res: Response) => {
  const user = await User.findById(req.user?.userId);

  if (user) {
    user.secretToken = null;
    user.tokenExpiresAt = null;
    user.trustedDevices = [];
    await user.save();
  }

  res.clearCookie('authToken');
  res.json({ success: true, message: 'Logout successful' });
});
// 8. Проверка авторизации
export const checkAuth = catchAsync(async (req: AuthRequest, res: Response) => {
  const user = await User.findById(req.user?.userId).select('-passwordHash');
  if (!user) {
    res.clearCookie('authToken');
    throw new NotFoundError('User', ErrorCodes.USER_NOT_FOUND);
  }
  res.json({
    success: true,
    user: {
      email: user?.email,
      status: user?.status,
      role: user?.role,          
      createdAt: user?.createdAt
    }
  });
});
// 9. Повторная отправка кода подтверждения
export const resendCode = catchAsync(async (req: Request, res: Response) => {
  const { email } = req.body;
  
  // Безопасность: не проверяем существование email
  // Всегда отвечаем одинаково
  const user = await User.findOne({ email });
  
  if (!user) {
    res.json({ message: 'Если email зарегистрирован, код будет отправлен' });
    return;
  }
  
  // Если email уже подтверждён
  if (user.status === 'active') {
    res.json({ message: 'Email уже подтверждён' });
    return;
  }
  
  // Проверяем, можно ли отправить код повторно максимум 3 попытки
  if (user.verificationAttempts >= 3) {
    // Удаляем мёртвую запись
    await User.deleteOne({ _id: user._id });
    res.json({ 
      message: 'Превышено количество попыток. Пожалуйста, зарегистрируйтесь заново.' 
    });
    return;
  }
  
  // Генерируем новый код (даже если старый ещё не истёк)
  const newCode = generateCode();
  user.verificationCode = newCode;
  user.verificationCodeExpiresAt = new Date(Date.now() + 15 * 60 * 1000);
  user.verificationAttempts += 1;
  await user.save();
  
  // Отправляем письмо
  try {
    await sendVerificationCode(email, newCode);
  } catch (error) {
    throw new EmailError('Не удалось отправить код', ErrorCodes.EMAIL_ERROR);
  }
  
  res.json({ message: 'Новый код подтверждения отправлен на email' });
});