# Auth Service - Инструкция по запуску и интеграции

## Что это?

Микросервис аутентификации для проекта "Работа не волк" (конструктор резюме).

**Возможности:**

- Регистрация и подтверждение email
- Логин/логаут с токенами на 7 дней
- Восстановление пароля
- Роли: user, premium, admin
- Множественные доверенные устройства
- Внутренний API для других микросервисов

---

## Требования

- **Node.js** 18+
- **MongoDB** (Docker или локально)
- **Gmail** для отправки писем (или другой SMTP)

---

## Установка и запуск

### 1. Клонируйте/скопируйте проект

```bash
cd auth-service
```

### 2. Установите зависимости

```bash
npm install
```

### 3. Настройте `.env`

Файл `.env` уже создан, проверьте/измените:

```bash
PORT=3000
MONGODB_URI=mongodb://localhost:27017/auth_db
EMAIL_USER=your-email@gmail.com          # ← Ваш Gmail
EMAIL_PASS=your-app-password             # ← App Password от Gmail
FRONTEND_URL=http://localhost:5173       # ← URL вашего фронтенда
INTERNAL_SECRET=secret-for-microservices # ← Секрет для микросервисов
ADMIN_EMAIL=admin@example.com            # ← Email админа
ADMIN_PASSWORD=securePassword123         # ← Пароль админа
```

**Как получить Gmail App Password:**

1. Google Account → Security → 2-Step Verification (включить)
2. App passwords → Generate → Скопировать 16-значный код
3. Вставить в `EMAIL_PASS`

### 4. Запустите MongoDB

**Docker (рекомендуется):**

```bash
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

**Локально:**

```bash
mongod --dbpath /path/to/data
```

### 5. Запустите сервис

**Разработка:**

```bash
npm run dev
```

**Продакшн:**

```bash
npm run build
npm start
```

### 6. Проверьте работу

```bash
curl http://localhost:3000/health
```

**Ответ:**

```json
{
  "status": "ok",
  "timestamp": "2026-04-11T12:16:12.917Z"
}
```

---

## Интеграция с фронтендом

### Настройка CORS

В `.env` укажите URL фронтенда:

```bash
FRONTEND_URL=http://localhost:5173
```

Сервис уже настроен на работу с этим URL.

### Пример запроса с фронта (Svelte/React/Vue)

```javascript
// Логин
const response = await fetch("http://localhost:3000/api/auth/login", {
  method: "POST",
  headers: { "Content-Type": "application/json" },
  credentials: "include", //  ОБЯЗАТЕЛЬНО! Для cookies
  body: JSON.stringify({
    email: "user@example.com",
    password: "password123",
  }),
});

const data = await response.json();
// Токен автоматически сохранён в HttpOnly cookie
```

**Важно:** Всегда используйте `credentials: 'include'` для отправки cookies!

### Проверка авторизации

```javascript
// При загрузке приложения
const response = await fetch("http://localhost:3000/api/auth/check", {
  credentials: "include",
});

if (response.ok) {
  const { user } = await response.json();
  // user: { email, role, status }
} else {
  // Перенаправить на /login
}
```

---

## Интеграция с другими микросервисами

### Архитектура

```
Frontend (Svelte)
    ↓ (с cookie authToken)
Order Service / Payment Service / etc.
    ↓ (валидация токена)
Auth Service (внутренний API)
```

### Пример: Order Service проверяет токен

```javascript
// order-service/middleware/auth.js
import fetch from "node-fetch";

export async function requireAuth(req, res, next) {
  const token = req.cookies.authToken;

  if (!token) {
    return res.status(401).json({ error: "No token" });
  }

  try {
    // Запрос к Auth Service
    const response = await fetch(
      "http://localhost:3000/api/internal/validate",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-Internal-Secret": "secret-for-microservices", // Из .env
        },
        body: JSON.stringify({ token }),
      },
    );

    const data = await response.json();

    if (data.valid) {
      req.user = data; // { userId, email, role, status }
      next();
    } else {
      res.status(401).json({ error: "Invalid token" });
    }
  } catch (error) {
    res.status(500).json({ error: "Auth check failed" });
  }
}

// Использование
app.post("/api/orders", requireAuth, (req, res) => {
  // req.user доступен здесь
  const userId = req.user.userId;
  // ...
});
```

### .env для других сервисов

```bash
# order-service/.env
PORT=3001
AUTH_SERVICE_URL=http://localhost:3000
INTERNAL_SECRET=secret-for-microservices  # ⚠️ Тот же, что в auth-service
```

---

## API Endpoints

### Публичные (для фронтенда)

| Метод | Endpoint                    | Описание                              |
| ----- | --------------------------- | ------------------------------------- |
| POST  | `/api/auth/register`        | Регистрация (отправляет код на email) |
| POST  | `/api/auth/verify`          | Подтверждение email с кодом           |
| POST  | `/api/auth/login`           | Вход (обновляет токен на 7 дней)      |
| POST  | `/api/auth/logout`          | Выход                                 |
| GET   | `/api/auth/check`           | Проверка авторизации                  |
| GET   | `/api/auth/profile`         | Данные профиля                        |
| POST  | `/api/auth/forgot-password` | Запрос кода сброса                    |
| POST  | `/api/auth/reset-password`  | Сброс пароля                          |
| POST  | `/api/auth/resend-code`     | Повторная отправка кода               |

### Защищённые (требуют авторизации)

| Метод | Endpoint                          | Роль           | Описание             |
| ----- | --------------------------------- | -------------- | -------------------- |
| GET   | `/api/auth/admin/users`           | admin          | Список пользователей |
| POST  | `/api/auth/admin/upgrade-premium` | admin          | Повысить до premium  |
| GET   | `/api/auth/premium/content`       | premium, admin | Премиум контент      |

### Внутренние (для микросервисов)

| Метод | Endpoint                   | Header            | Описание              |
| ----- | -------------------------- | ----------------- | --------------------- |
| POST  | `/api/internal/validate`   | X-Internal-Secret | Валидация токена      |
| POST  | `/api/internal/check-user` | X-Internal-Secret | Проверка пользователя |

**Пример запроса:**

```bash
curl -X POST http://localhost:3000/api/internal/validate \
  -H "Content-Type: application/json" \
  -H "X-Internal-Secret: secret-for-microservices" \
  -d '{"token":"abc123..."}'
```

**Ответ:**

```json
{
  "valid": true,
  "userId": "507f1f77bcf86cd799439011",
  "email": "user@example.com",
  "role": "user",
  "status": "active"
}
```

---

## 🗄️ База данных

### Структура

**Коллекция:** `users` в БД `auth_db`

**Основные поля:**

- `email` — уникальный
- `passwordHash` — bcrypt
- `status` — `pending` | `active`
- `role` — `user` | `premium` | `admin`
- `secretToken` — токен на 7 дней
- `tokenExpiresAt` — срок действия токена
- `trustedDevices[]` — массив устройств с fingerprint

**Автосоздание:**

- БД создаётся автоматически при первом запуске
- Индексы создаются автоматически
- Админ создаётся из `.env` (ADMIN_EMAIL, ADMIN_PASSWORD)

---

## 🧪 Тестирование

### Postman

1. Импортируйте `postman_collection.json` в Postman
2. Настройте переменные (email, password)
3. Включите cookies: Settings → Send cookies
4. Запустите тесты по порядку

**Подробнее:** см. `POSTMAN_GUIDE.md`

### Быстрый тест через curl

```bash
# 1. Регистрация
curl -X POST http://localhost:3000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password123"}'

# 2. Проверьте email, получите код

# 3. Подтверждение
curl -X POST http://localhost:3000/api/auth/verify \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","code":"123456"}' \
  -c cookies.txt

# 4. Проверка авторизации
curl -X GET http://localhost:3000/api/auth/check \
  -b cookies.txt
```

---

## Безопасность

### Что реализовано:

HttpOnly cookies (защита от XSS)  
 bcrypt для паролей (10 раундов)  
 Fingerprint устройства (User-Agent + Accept-Language + Accept-Encoding)  
 Токены истекают через 7 дней  
 Множественные доверенные устройства  
 Внутренний API защищён секретом  
 CORS настроен на конкретный фронтенд

### Рекомендации для продакшена:

- [ ] Используйте HTTPS (обязательно!)
- [ ] Смените `INTERNAL_SECRET` на сложный
- [ ] Настройте rate limiting (например, express-rate-limit)
- [ ] Добавьте мониторинг (Sentry, LogRocket)
- [ ] Используйте переменные окружения, не коммитьте `.env`
- [ ] Настройте MongoDB с аутентификацией
- [ ] Используйте Redis для сессий (опционально)

---

## 📂 Структура проекта

```
auth-service/
├── src/
│   ├── config/
│   │   └── database.ts          # Подключение к MongoDB
│   ├── controllers/
│   │   ├── authController.ts    # Логика регистрации/логина
│   │   ├── adminController.ts   # Админские функции
│   │   ├── premiumController.ts # Премиум контент
│   │   └── internalController.ts # Внутренний API
│   ├── middleware/
│   │   ├── auth.ts              # Проверка токена
│   │   ├── role.ts              # Проверка роли
│   │   └── validate.ts          # Валидация запросов
│   ├── models/
│   │   └── User.ts              # Модель пользователя
│   ├── routes/
│   │   ├── authRoutes.ts        # Публичные роуты
│   │   └── internalRoutes.ts    # Внутренние роуты
│   ├── utils/
│   │   ├── token.ts             # Генерация токенов
│   │   ├── fingerprint.ts       # Fingerprint устройства
│   │   └── email.ts             # Отправка писем
│   ├── errors/                  # Обработка ошибок
│   ├── scripts/
│   │   └── createAdmin.ts       # Создание админа
│   └── server.ts                # Точка входа
├── .env                         # Переменные окружения
├── package.json
├── tsconfig.json
├── postman_collection.json      # Тесты Postman
└── POSTMAN_GUIDE.md             # Инструкция по тестам
```

---

## Решение проблем

### MongoDB не подключается

```bash
# Проверьте, что MongoDB запущена
docker ps
# или
mongod --version

# Перезапустите контейнер
docker restart mongodb
```

### Email не отправляются

- Проверьте `EMAIL_USER` и `EMAIL_PASS` в `.env`
- Убедитесь, что используете App Password, а не обычный пароль
- Проверьте, что 2FA включена в Google Account

### Токен не сохраняется на фронте

- Убедитесь, что используете `credentials: 'include'`
- Проверьте CORS: `FRONTEND_URL` должен совпадать с URL фронта
- В продакшене используйте HTTPS

### "Device not trusted"

- Fingerprint привязан к User-Agent
- Не меняйте браузер/устройство между запросами
- Выполните логин заново на новом устройстве

---

## Контакты и поддержка

**Порты по умолчанию:**

- Auth Service: `3000`
- MongoDB: `27017`
- Frontend: `5173` (Svelte)

**Логи:**

- Сервер: консоль где запущен `npm run dev`
- MongoDB: `docker logs mongodb`

**Полезные команды:**

```bash
npm run dev      # Запуск в режиме разработки
npm run build    # Компиляция TypeScript
npm start        # Запуск продакшн версии
docker ps        # Проверка контейнеров
```

---

## Чеклист

- [ ] MongoDB запущена
- [ ] `.env` настроен (email, пароли, URL)
- [ ] Сервис запущен (`npm run dev`)
- [ ] Health check работает (`/health`)
- [ ] Админ создан (проверить логи)
- [ ] Postman тесты проходят
- [ ] Фронтенд может логиниться
- [ ] Другие микросервисы могут валидировать токены
