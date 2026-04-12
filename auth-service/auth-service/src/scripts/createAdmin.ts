import { User } from '../models/User.js';
import bcrypt from 'bcryptjs';
import dotenv from 'dotenv';

dotenv.config();

export const createAdminIfNotExists = async () => {
  const adminEmail = process.env.ADMIN_EMAIL || 'admin@example.com';
  const adminPassword = process.env.ADMIN_PASSWORD || 'admin123';

  const existingAdmin = await User.findOne({ role: 'admin' });

  if (!existingAdmin) {
    const passwordHash = await bcrypt.hash(adminPassword, 10);

    const admin = new User({
      email: adminEmail,
      passwordHash,
      status: 'active',
      role: 'admin',
      verificationCode: null,
      verificationCodeExpiresAt: null,
      secretToken: null,
      tokenExpiresAt: null,
      trustedDevices: [],
      verificationAttempts: 0,
      pendingExpiresAt: null
    });

    await admin.save();
    console.log(`Admin user created: ${adminEmail}`);
  } else {
    console.log('Admin already exists');
  }
};