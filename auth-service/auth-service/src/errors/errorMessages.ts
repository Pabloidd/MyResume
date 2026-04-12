// src/errors/errorMessages.ts

import type { ErrorCode } from './errorCodes.js';
import { ErrorCodes} from './errorCodes.js';
// Типизированный объект сообщений
export const ErrorMessages: Record<ErrorCode, string> = {
  // 400
  [ErrorCodes.MISSING_FIELD]: 'Отсутствует обязательное поле',
  [ErrorCodes.INVALID_EMAIL]: 'Неверный формат email',
  [ErrorCodes.PASSWORD_TOO_SHORT]: 'Пароль слишком короткий (минимум 6 символов)',
  [ErrorCodes.INVALID_CODE]: 'Неверный код',
  [ErrorCodes.CODE_EXPIRED]: 'Код истёк',
  
  // 401
  [ErrorCodes.NO_TOKEN]: 'Токен не предоставлен',
  [ErrorCodes.INVALID_TOKEN]: 'Неверный или просроченный токен',
  [ErrorCodes.TOKEN_EXPIRED]: 'Токен истёк, выполните вход заново',
  [ErrorCodes.DEVICE_MISMATCH]: 'Устройство не распознано, выполните вход заново',
  [ErrorCodes.INVALID_CREDENTIALS]: 'Неверный email или пароль',
  
  // 403
  [ErrorCodes.EMAIL_NOT_VERIFIED]: 'Email не подтверждён. Проверьте почту.',
  [ErrorCodes.ACCESS_DENIED]: 'Доступ запрещён',
  
  // 404
  [ErrorCodes.USER_NOT_FOUND]: 'Пользователь не найден',
  
  // 409
  [ErrorCodes.EMAIL_ALREADY_EXISTS]: 'Пользователь с таким email уже существует',
  
  // 500
  [ErrorCodes.DATABASE_ERROR]: 'Ошибка базы данных',
  [ErrorCodes.EMAIL_ERROR]: 'Не удалось отправить письмо',
  [ErrorCodes.INTERNAL_ERROR]: 'Внутренняя ошибка сервера'
};

// Функция получения сообщения по коду
export const getErrorMessage = (code: ErrorCode, params?: Record<string, string>): string => {
  let message = ErrorMessages[code] || 'Неизвестная ошибка';
  if (params) {
    Object.entries(params).forEach(([key, value]) => {
      message = message.replace(`{{${key}}}`, value);
    });
  }
  return message;
};