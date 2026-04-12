import type { Request } from 'express';

export interface UserPayload {
  userId: string;
  email: string;
  status: 'pending' | 'active';
  role: 'user' | 'premium' | 'admin';  
}

export interface AuthRequest extends Request {
  user?: UserPayload;
}

export interface RegisterBody {
  email: string;
  password: string;
}

export interface VerifyBody {
  email: string;
  code: string;
}

export interface LoginBody {
  email: string;
  password: string;
}

export interface ForgotPasswordBody {
  email: string;
}

export interface ResetPasswordBody {
  email: string;
  code: string;
  newPassword: string;
}