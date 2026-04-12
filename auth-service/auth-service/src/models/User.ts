import mongoose, { Schema, Document } from 'mongoose';

export interface ITrustedDevice {
  fingerprint: string;
  lastSeen: Date;
  deviceName: string;
}

export interface IUser extends Document {
  email: string;
  passwordHash: string;
  status: 'pending' | 'active';
  role: 'user' | 'premium' | 'admin';

  verificationCode: string | null;
  verificationCodeExpiresAt: Date | null;

  resetCode: string | null;
  resetCodeExpiresAt: Date | null;

  secretToken: string | null;
  tokenExpiresAt: Date | null;  // Токен истекает через 7 дней
  trustedDevices: ITrustedDevice[];

  verificationAttempts: number;
  pendingExpiresAt: Date | null;

  createdAt: Date;
  updatedAt: Date;
}

const TrustedDeviceSchema = new Schema<ITrustedDevice>({
  fingerprint: { type: String, required: true },
  lastSeen: { type: Date, default: Date.now },
  deviceName: { type: String, default: 'Unknown Device' }
}, { _id: false });

const UserSchema = new Schema<IUser>({
  email: {
    type: String,
    required: true,
    unique: true,
    lowercase: true
  },
  passwordHash: { type: String, required: true },
  status: {
    type: String,
    enum: ['pending', 'active'],
    default: 'pending'
  },
  role: {
    type: String,
    enum: ['user', 'premium', 'admin'],
    default: 'user'
  },

  verificationCode: { type: String, default: null },
  verificationCodeExpiresAt: { type: Date, default: null },

  resetCode: { type: String, default: null },
  resetCodeExpiresAt: { type: Date, default: null },

  secretToken: { type: String, default: null },
  tokenExpiresAt: { type: Date, default: null },
  trustedDevices: { type: [TrustedDeviceSchema], default: [] },

  verificationAttempts: { type: Number, default: 0 },
  pendingExpiresAt: { type: Date, default: null },

  createdAt: { type: Date, default: Date.now },
  updatedAt: { type: Date, default: Date.now }
}, {
  timestamps: true
});

UserSchema.index({ email: 1 });
UserSchema.index({ secretToken: 1 });
UserSchema.index({ verificationCode: 1 });
UserSchema.index({ resetCode: 1 });
UserSchema.index({ pendingExpiresAt: 1 }, { expireAfterSeconds: 0 });

export const User = mongoose.model<IUser>('User', UserSchema);