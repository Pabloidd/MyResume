export class AppError extends Error {
  public readonly statusCode: number;
  public readonly errorCode: string;
  public readonly isOperational: boolean;

  constructor(
    statusCode: number,
    message: string,
    errorCode: string = 'UNKNOWN_ERROR',
    isOperational: boolean = true
  ) {
    super(message);
    this.statusCode = statusCode;
    this.errorCode = errorCode;
    this.isOperational = isOperational;
    
    Error.captureStackTrace(this, this.constructor);
  }
}

// Специализированные ошибки
export class ValidationError extends AppError {
  constructor(message: string, errorCode: string = 'VALIDATION_ERROR') {
    super(400, message, errorCode);
  }
}

export class AuthenticationError extends AppError {
  constructor(message: string, errorCode: string = 'AUTH_ERROR') {
    super(401, message, errorCode);
  }
}

export class AuthorizationError extends AppError {
  constructor(message: string, errorCode: string = 'FORBIDDEN') {
    super(403, message, errorCode);
  }
}

export class NotFoundError extends AppError {
  constructor(resource: string, errorCode: string = 'NOT_FOUND') {
    super(404, `${resource} not found`, errorCode);
  }
}

export class ConflictError extends AppError {
  constructor(message: string, errorCode: string = 'CONFLICT') {
    super(409, message, errorCode);
  }
}

export class RateLimitError extends AppError {
  constructor(message: string = 'Too many requests', errorCode: string = 'RATE_LIMIT') {
    super(429, message, errorCode);
  }
}

export class DatabaseError extends AppError {
  constructor(message: string, errorCode: string = 'DATABASE_ERROR') {
    super(500, message, errorCode, true);
  }
}

export class EmailError extends AppError {
  constructor(message: string, errorCode: string = 'EMAIL_ERROR') {
    super(500, message, errorCode, true);
  }
}