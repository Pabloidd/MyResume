import type { Response } from 'express';
import type { AuthRequest } from '../types/index.js';
import { catchAsync } from '../errors/errorHandler.js';
import { AuthorizationError } from '../errors/AppError.js';
import { ErrorCodes } from '../errors/errorCodes.js';

// Премиум контент (доступен для premium и admin)
export const getPremiumContent = catchAsync(async (req: AuthRequest, res: Response) => {
  res.json({
    success: true,
    content: {
      title: 'Премиум контент',
      description: 'Этот контент доступен только пользователям с премиум-подпиской'
    }
  });
});