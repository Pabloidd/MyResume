<div class="feedback-page">
  <!-- Заголовок -->
  <div class="feedback-header">
    <h1 class="page-title">Обратная связь</h1>
    <p class="page-subtitle">Есть вопросы или предложения? Напишите нам!</p>
  </div>

  <div class="feedback-container">
    <!-- Форма обратной связи -->
    <div class="feedback-form">
        <div class="form-group">
          <label class="form-label">Имя</label>
          <input 
            type="text" 
            class="form-input" 
            placeholder="Как к вам обращаться?"
          >
        </div>
 
      
      <div class="form-group">
        <label class="form-label">Тема</label>
        <select class="form-select">
          <option value="" disabled selected>Выберите тему</option>
          <option value="question">Вопрос</option>
          <option value="bug">Нашел ошибку</option>
          <option value="suggestion">Предложение</option>
          <option value="cooperation">Сотрудничество</option>
          <option value="other">Другое</option>
        </select>
      </div>
      
      <div class="form-group">
        <label class="form-label">Сообщение</label>
        <textarea 
          class="form-textarea" 
          placeholder="Напишите подробно о вашем вопросе или предложении..."
          rows="5"
        ></textarea>
      </div>
      
      <button class="submit-btn">Отправить</button>
    </div>

    <!-- Контактная информация -->
    <div class="contact-info">
      <h2 class="info-title">Другие способы связи</h2>
      
      <div class="info-item">
        <span class="info-icon">📧</span>
        <div class="info-content">
          <span class="info-label">Email</span>
          <a href="mailto:support@myresume.com" class="info-link">support@myresume.com</a>
        </div>
      </div>
      
      <div class="info-item">
        <span class="info-icon">📱</span>
        <div class="info-content">
          <span class="info-label">Telegram</span>
          <a href="#" class="info-link">@myresume_support</a>
        </div>
      </div>
      
      <div class="info-item">
        <span class="info-icon">🕒</span>
        <div class="info-content">
          <span class="info-label">Время работы</span>
          <span class="info-text">Пн-Пт, 10:00 - 19:00</span>
        </div>
      </div>
      
      <div class="info-item">
        <span class="info-icon">📍</span>
        <div class="info-content">
          <span class="info-label">Адрес</span>
          <span class="info-text">г. Москва, ул. Программистов, д. 1</span>
        </div>
      </div>

      <div class="social-links">
        <h3 class="social-title">Мы в соцсетях</h3>
        <div class="social-icons">
          <a href="#" class="social-icon" aria-label="Telegram">📨</a>
          <a href="#" class="social-icon" aria-label="VK">📘</a>
          <a href="#" class="social-icon" aria-label="GitHub">💻</a>
        </div>
      </div>
    </div>
  </div>
</div>

<style>
  .feedback-page {
    max-width: 1200px;
    margin: 0 auto;
    padding: 2rem;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  }

  /* Заголовок */
  .feedback-header {
    margin-bottom: 3rem;
  }

  .page-title {
    font-size: 2.5rem;
    font-weight: bold;
    margin: 0 0 0.5rem 0;
    color: #000;
  }

  .page-subtitle {
    font-size: 1.25rem;
    color: #475569;
    margin: 0;
  }

  /* Контейнер */
  .feedback-container {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 3rem;
  }

  /* Форма */
  .feedback-form {
    background: white;
    padding: 1rem;
    border-radius: 20px;
    box-shadow: 0 8px 20px rgba(102, 155, 188, 0.15);
    border: 1px solid rgba(102, 155, 188, 0.2);
  }

  .form-row {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    margin-bottom: 1rem;
  }

  .form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    margin-bottom: 1.5rem;
  }

  .form-label {
    font-size: 1rem;
    font-weight: 600;
    color: #475569;
  }

  .form-input,
  .form-select,
  .form-textarea {
    padding: 0.75rem 1rem;
    font-size: 1rem;
    border: 2px solid #e2e8f0;
    border-radius: 8px;
    transition: all 0.2s;
    background: white;
    width: 100%;
    box-sizing: border-box;
  }

  .form-input:focus,
  .form-select:focus,
  .form-textarea:focus {
    outline: none;
    border-color: #2563eb;
    box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  }

  .form-select {
    appearance: none;
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23475569' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
    background-repeat: no-repeat;
    background-position: right 1rem center;
    background-size: 16px;
  }

  .form-textarea {
    resize: vertical;
    font-family: inherit;
  }

  .submit-btn {
    background-color: #2563eb;
    color: white;
    border: none;
    border-radius: 8px;
    padding: 0.75rem 2rem;
    font-size: 1.1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
    width: 100%;
  }

  .submit-btn:hover {
    background-color: #1d4ed8;
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(37, 99, 235, 0.3);
  }

  /* Контактная информация */
  .contact-info {
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    padding: 2rem;
    border-radius: 20px;
    border: 1px solid rgba(102, 155, 188, 0.2);
    height: fit-content;
  }

  .info-title {
    font-size: 1.5rem;
    font-weight: bold;
    margin: 0 0 1.5rem 0;
    color: #1e293b;
  }

  .info-item {
    display: flex;
    gap: 1rem;
    margin-bottom: 1.5rem;
  }

  .info-icon {
    font-size: 1.5rem;
    min-width: 2rem;
  }

  .info-content {
    display: flex;
    flex-direction: column;
  }

  .info-label {
    font-size: 0.9rem;
    color: #64748b;
    margin-bottom: 0.25rem;
  }

  .info-link {
    font-size: 1rem;
    color: #2563eb;
    text-decoration: none;
    transition: color 0.2s;
  }

  .info-link:hover {
    color: #1d4ed8;
    text-decoration: underline;
  }

  .info-text {
    font-size: 1rem;
    color: #1e293b;
  }

  /* Социальные сети */
  .social-links {
    margin-top: 2rem;
    padding-top: 1.5rem;
    border-top: 1px solid #e2e8f0;
  }

  .social-title {
    font-size: 1rem;
    font-weight: 600;
    color: #475569;
    margin: 0 0 1rem 0;
  }

  .social-icons {
    display: flex;
    gap: 1rem;
  }

  .social-icon {
    width: 40px;
    height: 40px;
    background: #f1f5f9;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.25rem;
    color: #475569;
    text-decoration: none;
    transition: all 0.2s;
  }

  .social-icon:hover {
    background: #2563eb;
    color: white;
    transform: translateY(-2px);
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  @media (max-width: 1300px) {
    .page-title {
      font-size: 2.2rem;
    }
  }

  @media (max-width: 1080px) {
    .page-title {
      font-size: 2rem;
    }
    
    .page-subtitle {
      font-size: 1.1rem;
    }
  }

  @media (max-width: 900px) {
    .feedback-container {
      grid-template-columns: 1fr;
    }
    
    .contact-info {
      order: -1;
    }
  }


  @media (max-width: 650px) {
    .feedback-page {
      padding: 0;
    }
    
    .page-title {
      font-size: 1.5rem;
    }
    
    .page-subtitle {
      font-size: 1rem;
    }
    
    .form-row {
      grid-template-columns: 1fr;
      gap: 0;
    }
    
    .feedback-form,
    .contact-info {
      padding: 0.75rem;
    }
    
    .info-title {
      font-size: 1.3rem;
    }
    
    .info-item {
      margin-bottom: 1rem;
    }
  }


  @media (max-width: 475px) {
    .page-title {
      font-size: 1.5rem;
    }
    
    .page-subtitle {
      font-size: 0.9rem;
    }
    
    .form-input,
    .form-select,
    .form-textarea {
      padding: 0.6rem 0.8rem;
      font-size: 0.9rem;
    }
    
    .submit-btn {
      font-size: 1rem;
    }
    
    .info-icon {
      font-size: 1.25rem;
    }
    
    .info-link,
    .info-text {
      font-size: 0.9rem;
    }
    
    .social-icon {
      width: 36px;
      height: 36px;
      font-size: 1rem;
    }
  }
</style>