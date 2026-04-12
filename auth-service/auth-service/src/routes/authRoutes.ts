import express from 'express';
import { validate, required, isEmail, minLength } from '../middleware/validate.js';
import {
  register,
  verify,
  login,
  profile,
  forgotPassword,
  resetPassword,
  logout,
  checkAuth,
  resendCode
} from '../controllers/authController.js';
import { requireAuth } from '../middleware/auth.js';
import { requireRole } from '../middleware/role.js';
import { getUsers, changeRole } from '../controllers/adminController.js';
import { getPremiumContent } from '../controllers/premiumController.js';

const router = express.Router();

// Публичные роуты
router.post('/register',
  validate([
    required('email'),
    isEmail,
    required('password'),
    minLength('password', 6)
  ]),
  register
);

router.post('/verify',
  validate([
    required('email'),
    required('code')
  ]),
  verify
);

router.post('/login',
  validate([
    required('email'),
    required('password')
  ]),
  login
);

router.post('/forgot-password',
  validate([required('email')]),
  forgotPassword
);

router.post('/reset-password',
  validate([
    required('email'),
    required('code'),
    required('newPassword'),
    minLength('newPassword', 6)
  ]),
  resetPassword
);

router.post('/resend-code',
  validate([required('email')]),
  resendCode
);

// Защищённые роуты (требуют токен)
router.get('/profile', requireAuth, profile);
router.get('/check', requireAuth, checkAuth);
router.post('/logout', requireAuth, logout);

// Только для админов
router.get('/admin/users', requireAuth, requireRole(['admin']), getUsers);
router.post('/admin/change-role',
  requireAuth,
  requireRole(['admin']),
  validate([required('userId'), required('newRole')]),
  changeRole
);

// Для админов и премиум
router.get('/premium/content', requireAuth, requireRole(['admin', 'premium']), getPremiumContent);
export default router;