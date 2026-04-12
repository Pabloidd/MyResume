import type { Request } from 'express';
import crypto from 'crypto';

export const createFingerprint = (req: Request): string => {
  const data = {
    userAgent: req.headers['user-agent'] || 'unknown',
    acceptLanguage: req.headers['accept-language'] || 'unknown',
    acceptEncoding: req.headers['accept-encoding'] || 'unknown',
  };
  
  return crypto
    .createHash('sha256')
    .update(JSON.stringify(data))
    .digest('hex');
};