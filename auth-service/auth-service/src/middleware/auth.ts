import type { Response, NextFunction } from 'express';
import { User } from '../models/User.js';
import { createFingerprint } from '../utils/fingerprint.js';
import { isTokenExpired } from '../utils/token.js';
import type { AuthRequest, UserPayload } from '../types/index.js';


export const requireAuth = async (
  req: AuthRequest,
  res: Response,
  next: NextFunction
): Promise<void> => {
  try {
    const token = req.cookies?.authToken;

    if (!token) {
      res.status(401).json({ error: 'No token provided' });
      return;
    }

    const user = await User.findOne({ secretToken: token });

    if (!user) {
      res.status(401).json({ error: 'Invalid token' });
      return;
    }

    // Проверяем срок действия токена
    if (isTokenExpired(user.tokenExpiresAt)) {
      res.status(401).json({ error: 'Token expired' });
      return;
    }

    const currentFingerprint = createFingerprint(req);
    const deviceExists = user.trustedDevices.some(
      d => d.fingerprint === currentFingerprint
    );

    if (!deviceExists) {
      res.status(401).json({ error: 'Device not trusted' });
      return;
    }

    // Обновляем lastSeen для текущего устройства
    const device = user.trustedDevices.find(d => d.fingerprint === currentFingerprint);
    if (device) {
      device.lastSeen = new Date();
      await user.save();
    }

    req.user = {
      userId: user._id.toString(),
      email: user.email,
      status: user.status,
      role: user.role
    };

    next();
  } catch (error) {
    console.error(error);
    res.status(500).json({ error: 'Auth check failed' });
  }
};