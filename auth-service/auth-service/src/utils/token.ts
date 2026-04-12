import crypto from 'crypto';

export const generateCode = (): string => {
  return Math.floor(100000 + Math.random() * 900000).toString();
};

export const generateToken = (): string => {
  return crypto.randomBytes(32).toString('hex');
};

export const isTokenExpired = (expiresAt: Date | null): boolean => {
  if (!expiresAt) return true;
  return new Date() > new Date(expiresAt);
};