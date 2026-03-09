<div class="upgrade-page">
  <!-- Заголовок -->
  <div class="page-header">
    <h1 class="page-title">Повышение статуса</h1>
    <p class="page-subtitle">Откройте все возможности платформы</p>
  </div>

  <!-- Текущий статус -->
  <div class="current-status">
    <span class="status-label">Ваш текущий статус:</span>
    <span class="status-badge">СТАНДАРТ</span>
  </div>

  <div class="premium-showcase">
    <div class="premium-card">
      <div class="premium-left">
        <h2 class="premium-name">Премиум</h2>
        <div class="premium-price">
          <span class="price">990 ₽</span>
          <span class="price-period">/месяц</span>
        </div>
        
        <ul class="premium-features">
          <li class="feature-item">✦ VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK</li>
          <li class="feature-item">✦ GHJKKL;SNJ FBJFDJS SAKDFKL GFGHHJH</li>
          <li class="feature-item">✦ NSKLKLKLKLKSKS VHBJKL;'GDKDJK DBJJJKF</li>
          <li class="feature-item">✦ FBJFDJS SAKDFKL GHJKKL;SNJ FDJDJ;F HJJHKK</li>
          <li class="feature-item">✦ SKFNKLK NSKLKLKLKLKSKS ×2</li>
          <li class="feature-item">✦ Приоритетная поддержка 24/7</li>
        </ul>
      </div>
      
      <div class="premium-right">
        <div class="premium-badge">🔥 Максимум возможностей</div>
        <p class="premium-description">
          VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK NSKLKLKLKLKSKS. 
          GHJKKL;SNJ FBJFDJS SAKDFKL GFGHHJH FDJDJ;F HJJHKK.
        </p>
      </div>
    </div>
  </div>

  <!-- Форма оплаты -->
  <div class="payment-section">
    <h2 class="section-title">Оплата</h2>
    
    <div class="payment-container">
      <div class="payment-total-simple">
        <span class="total-label">К оплате:</span>
        <span class="total-amount">990 ₽</span>
      </div>

      <div class="payment-options">
        <a href="#" class="payment-link" on:click|preventDefault={() => alert('Ввод промокода...')}>
          <span class="link-icon">🎟️</span>
          Оплатить промокодом
        </a>
        <a href="#" class="payment-link" on:click|preventDefault={() => alert('Перенаправление на СБП...')}>
          <span class="link-icon">🏦</span>
          Оплатить через СБП
        </a>
        <a href="#" class="payment-link" on:click|preventDefault={() => alert('Перенаправление на ЮMoney...')}>
          <span class="link-icon">⭐</span>
          Оплатить через ЮMoney
        </a>
      </div>
      
      <p class="payment-note">
        Нажимая "Оплатить", вы соглашаетесь с условиями оферты и политикой обработки данных
      </p>
    </div>
  </div>

  <!-- Что дает премиум (детали) -->
  <section class="details-section">
    <h2 class="section-title">Подробнее о возможностях</h2>
    
    <div class="details-grid">
      <div class="detail-item">
        <h3 class="detail-title">VHBJKL;'GDKDJK</h3>
        <p class="detail-text">DBJJJKF FKHJDSKL SKFNKLK NSKLKLKLKLKSKS. GHJKKL;SNJ FBJFDJS SAKDFKL GFGHHJH FDJDJ;F HJJHKK.</p>
      </div>
      
      <div class="detail-item">
        <h3 class="detail-title">GHJKKL;SNJ FBJFDJS</h3>
        <p class="detail-text">SAKDFKL GFGHHJH FDJDJ;F HJJHKK. VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK NSKLKLKLKLKSKS.</p>
      </div>
      
      <div class="detail-item">
        <h3 class="detail-title">GFGHHJH FDJDJ;F</h3>
        <p class="detail-text">HJJHKK VHBJKL;'GDKDJK DBJJJKF FKHJDSKL. SKFNKLK NSKLKLKLKLKSKS GHJKKL;SNJ FBJFDJS SAKDFKL.</p>
      </div>
      
      <div class="detail-item">
        <h3 class="detail-title">NSKLKLKLKLKSKS</h3>
        <p class="detail-text">VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK. GHJKKL;SNJ FBJFDJS SAKDFKL GFGHHJH FDJDJ;F HJJHKK.</p>
      </div>
    </div>
  </section>
</div>

<style>
  .upgrade-page {
    max-width: 1000px;
    margin: 0 auto;
    padding: 2rem;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  }

  /* Заголовок */
  .page-header {
    margin-bottom: 2rem;
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

  /* Текущий статус */
  .current-status {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-bottom: 3rem;
    padding: 1rem 1.5rem;
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 12px;
    border: 1px solid rgba(102, 155, 188, 0.2);
  }

  .status-label {
    font-size: 1.1rem;
    color: #475569;
  }

  .status-badge {
    background: #e0f2fe;
    color: #0369a1;
    padding: 0.35rem 1rem;
    border-radius: 999px;
    font-size: 1rem;
    font-weight: 600;
    letter-spacing: 0.5px;
  }

  /* Премиум карточка */
  .premium-showcase {
    margin-bottom: 3rem;
  }

  .premium-card {
    background: linear-gradient(135deg, #f0f9ff 0%, #e6f0f5 100%);
    border-radius: 24px;
    padding: 2rem;
    border: 2px solid #2563eb;
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 2rem;
  }

  .premium-name {
    font-size: 2rem;
    font-weight: bold;
    color: #1e293b;
    margin: 0 0 0.5rem 0;
  }

  .premium-price {
    margin-bottom: 1.5rem;
  }

  .price {
    font-size: 2.5rem;
    font-weight: bold;
    color: #2563eb;
  }

  .price-period {
    font-size: 1rem;
    color: #64748b;
    margin-left: 0.25rem;
  }

  .premium-features {
    list-style: none;
    padding: 0;
    margin: 0;
  }

  .feature-item {
    padding: 0.5rem 0;
    font-size: 1rem;
    color: #475569;
    border-bottom: 1px dashed rgba(102, 155, 188, 0.3);
  }

  .feature-item:last-child {
    border-bottom: none;
  }

  .premium-right {
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

  .premium-badge {
    display: inline-block;
    background: #2563eb;
    color: white;
    padding: 0.25rem 1rem;
    border-radius: 20px;
    font-size: 0.9rem;
    font-weight: 600;
    margin-bottom: 1rem;
    width: fit-content;
  }

  .premium-description {
    font-size: 1rem;
    color: #475569;
    line-height: 1.6;
    margin: 0;
  }

  /* Секция оплаты */
  .payment-section {
    margin-bottom: 4rem;
  }

  .section-title {
    font-size: 1.5rem;
    font-weight: bold;
    margin: 0 0 2rem 0;
    color: #000;
  }

  .payment-container {
    background: white;
    border-radius: 20px;
    padding: 2rem;
    box-shadow: 0 8px 20px rgba(102, 155, 188, 0.15);
    border: 1px solid rgba(102, 155, 188, 0.2);
  }

  .payment-subtitle {
    font-size: 1.2rem;
    font-weight: 600;
    color: #1e293b;
    margin: 0 0 1rem 0;
  }

  /* Оплата */
  .payment-total-simple {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 2rem;
    padding-bottom: 1.5rem;
    border-bottom: 2px dashed #e2e8f0;
  }

  .total-label {
    font-size: 1.5rem;
    font-weight: 600;
    color: #475569;
  }

  .total-amount {
    font-size: 2.5rem;
    font-weight: bold;
    color: #1e293b;
  }

  .payment-options {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    margin-bottom: 2rem;
  }

  .payment-link {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 1rem;
    padding: 1rem;
    background: #f8fafc;
    border: 2px solid #e2e8f0;
    border-radius: 12px;
    color: #1e293b;
    font-size: 1.1rem;
    font-weight: 600;
    text-decoration: none;
    transition: all 0.2s;
  }

  .payment-link:hover {
    border-color: #2563eb;
    background: #f0f9ff;
    transform: translateY(-2px);
  }

  .link-icon {
    font-size: 1.5rem;
  }

  .payment-note {
    font-size: 0.85rem;
    color: #94a3b8;
    text-align: center;
    margin: 0;
  }

  /* Детали */
  .details-section {
    margin-bottom: 2rem;
  }

  .details-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
  }

  .detail-item {
    background: #f8fafc;
    padding: 1.5rem;
    border-radius: 12px;
    border-left: 4px solid #2563eb;
  }

  .detail-title {
    font-size: 1.2rem;
    font-weight: bold;
    margin: 0 0 0.5rem 0;
    color: #1e293b;
  }

  .detail-text {
    font-size: 0.95rem;
    color: #475569;
    margin: 0;
    line-height: 1.5;
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  /* 900px */
  @media (max-width: 900px) {
    .premium-card {
      grid-template-columns: 1fr;
      gap: 1.5rem;
    }
    
    .methods-grid {
      grid-template-columns: 1fr;
    }
    
    .details-grid {
      grid-template-columns: 1fr;
    }
  }

  /* 650px */
  @media (max-width: 650px) {
    .payment-total-simple {
      flex-direction: column;
      align-items: flex-start;
      gap: 0.5rem;
    }

    .premium-name {
      font-size: 1.5rem;
    }
    
    .price {
      font-size: 2rem;
    }
    
    .payment-container {
      padding: 1.5rem;
    }
    
    .form-row {
      grid-template-columns: 1fr;
    }
    
    .total-amount {
      font-size: 2rem;
    }
    
    .payment-link {
      font-size: 1rem;
      padding: 0.875rem;
    }
  }

  /* 475px */
  @media (max-width: 475px) {
    .page-title {
      font-size: 1.5rem;
    }
    
    .page-subtitle {
      font-size: 0.9rem;
    }
    
    .premium-card {
      padding: 1.5rem;
    }
    
    .price {
      font-size: 1.75rem;
    }
    
    .feature-item {
      font-size: 0.9rem;
    }
    
    .payment-subtitle {
      font-size: 1.1rem;
    }
    
    .method-content {
      padding: 0.75rem;
    }
    
    .method-name {
      font-size: 0.9rem;
    }
    
    .form-input {
      padding: 0.6rem 0.8rem;
      font-size: 0.9rem;
    }
    
    .detail-item {
      padding: 1rem;
    }
    
    .detail-title {
      font-size: 1rem;
    }
    
    .detail-text {
      font-size: 0.85rem;
    }
  }
</style>