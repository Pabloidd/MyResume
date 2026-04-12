import express from 'express';
import { validateToken, checkUserExists } from '../controllers/internalController.js';

const router = express.Router();

router.post('/validate', validateToken);
router.post('/check-user', checkUserExists);

export default router;