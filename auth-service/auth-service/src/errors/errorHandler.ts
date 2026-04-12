import type { Request, Response, NextFunction } from 'express';
import { AppError } from './AppError.js';
import { ErrorCodes } from './errorCodes.js';

// Обёртка для контроллеров (убирает try/catch)
export const catchAsync = (fn: Function) => {
  return (req: Request, res: Response, next: NextFunction) => {
    fn(req, res, next).catch(next);
  };
};

// Централизованный обработчик ошибок
export const errorHandler = (err: Error | AppError, req: Request, res: Response, next: NextFunction) => {
  console.error(`[${new Date().toISOString()}] ERROR:`, err.message);
  
  // Известная ошибка
  if (err instanceof AppError) {
    res.status(err.statusCode).json({
      success: false,
      error: {
        code: err.errorCode,
        message: err.message
      }
    });
    return;
  }
  
  // Ошибка MongoDB дубликат (email уже существует)
  if (err.name === 'MongoServerError' && (err as any).code === 11000) {
    res.status(409).json({
      success: false,
      error: {
        code: ErrorCodes.EMAIL_ALREADY_EXISTS,
        message: 'Пользователь с таким email уже существует'
      }
    });
    return;
  }
  
  // Неизвестная ошибка
  res.status(500).json({
    success: false,
    error: {
      code: 'INTERNAL_ERROR',
      message: 'Внутренняя ошибка сервера'
    }
  });
};