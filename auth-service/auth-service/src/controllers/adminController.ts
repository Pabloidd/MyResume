import type { Response } from 'express';
import { User } from '../models/User.js';
import type { AuthRequest } from '../types/index.js';
import { catchAsync } from '../errors/errorHandler.js';
import { AuthorizationError, NotFoundError } from '../errors/AppError.js';
import { ErrorCodes } from '../errors/errorCodes.js';

// Получить всех пользователей для admin
export const getUsers = catchAsync(async (req: AuthRequest, res: Response) => {
  const users = await User.find({}).select('-passwordHash -token -fingerprint');

  res.json({
    success: true,
    users
  });
});

// Изменить роль пользователя (только для admin)
export const changeRole = catchAsync(async (req: AuthRequest, res: Response) => {
  const { userId, newRole } = req.body;

  if (!userId || !newRole) {
    throw new NotFoundError('User ID and Role required', ErrorCodes.MISSING_FIELD);
  }

  const validRoles = ['user', 'premium', 'admin'];
  if (!validRoles.includes(newRole)) {
    throw new AuthorizationError('Invalid role', ErrorCodes.INVALID_FIELD);
  }

  const user = await User.findById(userId);

  if (!user) {
    throw new NotFoundError('User', ErrorCodes.USER_NOT_FOUND);
  }

  // Защита от изменения роли самого себя или других админов (опционально, но безопасно)
  if (user.role === 'admin' && req.user?.role !== 'admin') {
     throw new AuthorizationError('Cannot modify admin role', ErrorCodes.ACCESS_DENIED);
  }

  user.role = newRole;
  await user.save();

  res.json({
    success: true,
    message: `User role updated to ${newRole}`,
    user: {
      email: user.email,
      role: user.role
    }
  });
});

/** Сводка по пользователям (MongoDB) для админ-панели */
export const getStats = catchAsync(async (_req: AuthRequest, res: Response) => {
  const [byRole, byStatus, totalUsers] = await Promise.all([
    User.aggregate<{ _id: string; count: number }>([
      { $group: { _id: '$role', count: { $sum: 1 } } }
    ]),
    User.aggregate<{ _id: string; count: number }>([
      { $group: { _id: '$status', count: { $sum: 1 } } }
    ]),
    User.countDocuments()
  ]);

  const usersByRole = { user: 0, premium: 0, admin: 0 };
  for (const row of byRole) {
    const key = row._id as keyof typeof usersByRole;
    if (key in usersByRole) usersByRole[key] = row.count;
  }

  const usersByStatus = { active: 0, pending: 0 };
  for (const row of byStatus) {
    const key = row._id as keyof typeof usersByStatus;
    if (key in usersByStatus) usersByStatus[key] = row.count;
  }

  res.json({
    success: true,
    stats: {
      totalUsers,
      usersByRole,
      usersByStatus
    }
  });
});