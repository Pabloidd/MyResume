// src/controllers/internalController.ts

import type{ Request, Response } from 'express';
import { User } from '../models/User.js';
import { isTokenExpired } from '../utils/token.js';
import { catchAsync } from '../errors/errorHandler.js';
import { ErrorCodes } from '../errors/errorCodes.js';

export const validateToken = catchAsync(async (req: Request, res: Response) => {
  const { token } = req.body;
  const internalSecret = req.headers['x-internal-secret'];

  // Проверка внутреннего секрета (доступ только у доверенных сервисов)
  if (internalSecret !== process.env.INTERNAL_SECRET) {
    return res.status(403).json({
      success: false,
      error: {
        code: 'FORBIDDEN',
        message: 'Invalid internal secret'
      }
    });
  }

  if (!token) {
    return res.status(400).json({
      success: false,
      error: {
        code: ErrorCodes.MISSING_FIELD,
        message: 'Token required'
      }
    });
  }

  // Ищем пользователя по secretToken
  const user = await User.findOne({ secretToken: token });

  if (!user) {
    return res.json({ valid: false });
  }

  // Проверяем срок действия токена
  if (isTokenExpired(user.tokenExpiresAt)) {
    return res.json({ valid: false });
  }

  // возвращаем данные пользователя
  res.json({
    valid: true,
    userId: user._id,
    email: user.email,
    role: user.role,
    status: user.status
  });
});

export const checkUserExists = catchAsync(async (req: Request, res: Response) => {
  const { email } = req.body;
  const internalSecret = req.headers['x-internal-secret'];
  
  if (internalSecret !== process.env.INTERNAL_SECRET) {
    return res.status(403).json({ 
      success: false,
      error: {
        code: 'FORBIDDEN',
        message: 'Invalid internal secret'
      }
    });
  }
  
  const user = await User.findOne({ email }).select('email role status');
  
  res.json({
    exists: !!user,
    user: user ? {
      email: user.email,
      role: user.role,
      status: user.status
    } : null
  });
});