import nodemailer from 'nodemailer';
import dotenv from 'dotenv';

dotenv.config();

const transporter = nodemailer.createTransport({
  service: 'gmail',
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASS
  }
});

export const sendVerificationCode = async (email: string, code: string): Promise<void> => {
  const mailOptions = {
    from: process.env.EMAIL_USER,
    to: email,
    subject: 'Подтверждение регистрации',
    html: `
      <h2>Подтверждение email</h2>
      <p>Ваш код подтверждения:</p>
      <h1 style="color: #667eea; font-size: 32px;">${code}</h1>
      <p>Код действителен 15 минут.</p>
    `
  };
  
  await transporter.sendMail(mailOptions);
};

export const sendResetCode = async (email: string, code: string): Promise<void> => {
  const mailOptions = {
    from: process.env.EMAIL_USER,
    to: email,
    subject: 'Восстановление пароля',
    html: `
      <h2>Сброс пароля</h2>
      <p>Ваш код для сброса пароля:</p>
      <h1 style="color: #667eea; font-size: 32px;">${code}</h1>
      <p>Код действителен 15 минут.</p>
    `
  };
  
  await transporter.sendMail(mailOptions);
};