import mongoose from 'mongoose';
import dotenv from 'dotenv';

dotenv.config();

export const connectDB = async (): Promise<void> => {
  try {
    const mongoUri = process.env.MONGODB_URI || 'mongodb://localhost:27017/auth_db';

    await mongoose.connect(mongoUri);

    console.log('MongoDB connected successfully');

    if (mongoose.connection.db) {
      console.log(`Database: ${mongoose.connection.db.databaseName}`);

      // Создаём индексы при первом подключении
      await mongoose.connection.db.collection('users').createIndexes([
        { key: { email: 1 }, unique: true, name: 'email_1' },
        { key: { secretToken: 1 }, name: 'secretToken_1' },
        { key: { verificationCode: 1 }, name: 'verificationCode_1' },
        { key: { resetCode: 1 }, name: 'resetCode_1' }
      ]);

      console.log('Database indexes created');
    }

  } catch (error) {
    console.error('MongoDB connection error:', error);
    console.error('Убедитесь, что MongoDB запущена:');
    console.error('   - Docker: docker run -d -p 27017:27017 --name mongodb mongo:latest');
    console.error('   - Локально: mongod --dbpath /path/to/data');
    process.exit(1);
  }
};