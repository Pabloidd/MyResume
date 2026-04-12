import type { Response, NextFunction } from 'express';
import type { AuthRequest } from '../types/index.js';
import { AuthorizationError } from '../errors/AppError.js';
import { ErrorCodes } from '../errors/errorCodes.js';

export const requireRole = (roles: string[]) => {
  return (req: AuthRequest, res: Response, next: NextFunction) => {
    if (!req.user) {
      return res.status(401).json({ error: 'Unauthorized' });
    }
    
    if (!roles.includes(req.user.role)) {
      throw new AuthorizationError('Access denied', ErrorCodes.ACCESS_DENIED);
    }
    
    next();
  };
};