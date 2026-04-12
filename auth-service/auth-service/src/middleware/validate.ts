import type{ Request, Response, NextFunction } from 'express';
import { ValidationError } from '../errors/AppError.js';
import { ErrorCodes } from '../errors/errorCodes.js';

export type Rule = {
  field: string;
  check: (value: any) => boolean;
  errorCode: string;
  message: string;
};

export const validate = (rules: Rule[]) => {
  return (req: Request, res: Response, next: NextFunction) => {
    for (const rule of rules) {
      const value = req.body[rule.field];
      if (!rule.check(value)) {
        return next(new ValidationError(rule.message, rule.errorCode));
      }
    }
    next();
  };
};

// Готовые проверки
export const required = (field: string): Rule => ({
  field,
  check: (v: any) => v !== undefined && v !== null && v !== '',
  errorCode: ErrorCodes.MISSING_FIELD,
  message: `Поле "${field}" обязательно`
});

export const isEmail: Rule = {
  field: 'email',
  check: (v: any) => typeof v === 'string' && /^\S+@\S+\.\S+$/.test(v),
  errorCode: ErrorCodes.INVALID_EMAIL,
  message: 'Неверный формат email'
};

export const minLength = (field: string, min: number): Rule => ({
  field,
  check: (v: any) => typeof v === 'string' && v.length >= min,
  errorCode: ErrorCodes.PASSWORD_TOO_SHORT,
  message: `Поле "${field}" должно содержать минимум ${min} символов`
});