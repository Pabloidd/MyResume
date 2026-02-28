<script>
    import { goto } from '$app/navigation';
  import ResumeRow from './ResumeRow.svelte';
    import { base } from '$app/paths';

  function goToCreate(){
    goto(`${base}/profile/create`);
  }

  function goToPremium(){
    goto(`${base}/profile/premium/`)
  }

function goToRestorePassword(){
    goto(`${base}/SignIn/restorePassword`)
  }
</script>

<!-- Заголовок и приветствие -->
<div class="page-header">
  <div class="header-top">
    <h1 class="page-title">Личный кабинет</h1>
    <button on:click={goToCreate} class="create-resume-btn">Создать резюме</button>
  </div>
  <p class="welcome-message">ДОБРО ПОЖАЛОВАТЬ!</p>
</div>

<!-- Информация о пользователе-->
<div class="user-info-section">
  <!-- Логин -->
  <div class="info-row">
    <span class="info-label">Логин:</span>
    <span class="info-value">DFHJG@GMAIL.COM</span>
  </div>
  
  <!-- Сменить пароль -->
  <div class="info-row">
    <button type="button" on:click={goToRestorePassword} class="action-link change-password">Сменить пароль</button>
  </div>
  
  <!-- Статус -->
  <div class="info-row">
    <span class="info-label">Статус:</span>
    <div class="status-wrapper">
      <span class="status-badge">СТАНДАРТ</span>
    </div>
  </div>
  
  <!-- Повысить статус -->
  <div class="info-row">
    <button on:click={goToPremium} class="action-link upgrade-status">Повысить статус</button>
  </div>
</div>

<!-- Доступно в статусе стандарт -->
<section class="features-section">
  <h3 class="features-title">В статусе стандарт вам доступно следующие:</h3>
  <ul class="features-list">
    <li>VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK NSKLKLKLKLKSKS</li>
    <li>GHJKKL;SNJ FBJFDJS SAKDFKL</li>
    <li>GFGHHJH FDJDJ;F HJJHKK</li>
  </ul>
</section>

<!-- Премиум предложение -->
<section class="premium-section">
  <h3 class="premium-title">Повысить статус до премиума! Вам откроются новые возможности:</h3>
  <ul class="premium-list">
    <li>VHBJKL;'GDKDJK DBJJJKF FKHJDSKL SKFNKLK NSKLKLKLKLKSKS</li>
    <li>GHJKKL;SNJ FBJFDJS SAKDFKL</li>
    <li>GFGHHJH FDJDJ;F HJJHKK</li>
  </ul>
</section>

<!-- Мои резюме -->
<section class="resumes-section">
  <h2 class="section-title">Мои резюме</h2>
  
  <!-- Заголовки таблицы -->
  <div class="resumes-header">
    <span class="resume-title-header">Название резюме</span>
    <span class="resume-status-header">Статус</span>
    <span class="resume-actions-header">Действия</span>
  </div>

  <!-- Строки резюме -->
  <ResumeRow 
    _title="Инженер-программист" 
    initialStatus="public"
  />
  
  <ResumeRow 
    _title="Менеджер проектов" 
    initialStatus="private"
  />
  
  <ResumeRow 
    _title="Дизайнер" 
    initialStatus="public"
  />
</section>

<style>
  /* Базовые стили (до 1300px) */
  .page-header {
    margin-bottom: 2.5rem;
  }

  .header-top {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 0.5rem;
  }

  .page-title {
    font-size: 2.5rem;
    font-weight: bold;
    margin: 0;
    color: #000;
  }

  .welcome-message {
    font-size: 1.75rem;
    font-weight: bold;
    color: rgba(0, 0, 0, 0.8);
    margin: 0;
  }

  /* Кнопка Создать резюме (как кнопка Выйти) */
  .create-resume-btn {
    background-color: rgba(193, 18, 31, 0.63);
    border-radius: 8px;
    margin: 0rem 3rem 0 0;
    padding: 0.5rem 1.5rem;
    color: rgba(255, 255, 255, 1);
    font-size: 1.75rem;
    font-weight: 550;
    cursor: pointer;
    transition: all 0.2s;
    border: none;
  }

  .create-resume-btn:hover {
    background-color: rgba(255, 0, 17, 0.63);
    transform: translateY(-1px);
  }

  .user-info-section {
    margin-bottom: 3rem;
    padding: 2rem;
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 20px;
    box-shadow: 0 8px 20px rgba(102, 155, 188, 0.15);
    border: 1px solid rgba(102, 155, 188, 0.2);
    display: flex;
    flex-direction: column;
    gap: 1rem;
  }

  .info-row {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.5rem 0;
    border-bottom: 1px dashed rgba(102, 155, 188, 0.2);
  }

  .info-row:last-child {
    border-bottom: none;
  }

  .info-label {
    min-width: 100px;
    font-size: 1.75rem;
    font-weight: 600;
    color: #475569;
  }

  .info-value {
    font-size: 1.35rem;
    font-weight: 500;
    color: #0f172a;
    background: white;
    padding: 0.5rem 1rem;
    border-radius: 12px;
    border: 1px solid #e2e8f0;
  }

  .action-link {
    background: none;
    border: 1px solid #e2e8f0;
    font-size: 1.35rem;
    color: #2563eb;
    cursor: pointer;
    padding: 0.5rem 1rem;
    border-radius: 12px;
    transition: all 0.2s;
    font-weight: 500;
    background: white;
  }

  .action-link:hover {
    color: #1d4ed8;
    background: #f8fafc;
    border-color: #2563eb;
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(37, 99, 235, 0.1);
  }

  .status-wrapper {
    background: white;
    padding: 0.5rem 1rem;
    border-radius: 12px;
    border: 1px solid #e2e8f0;
  }

  .status-badge {
    background: #e0f2fe;
    color: #0369a1;
    padding: 0.35rem 1rem;
    border-radius: 999px;
    font-size: 1.35rem;
    font-weight: 600;
    letter-spacing: 0.5px;
    display: inline-block;
  }

  .features-section,
  .premium-section {
    margin-bottom: 2.5rem;
    padding: 2rem;
    background: linear-gradient(135deg, #f0f9ff 0%, #e6f0f5 100%);
    border-radius: 16px;
    border-left: 4px solid #669bbc;
  }

  .premium-section {
    background: linear-gradient(135deg, #fff1f0 0%, #fee9e7 100%);
    border-left-color: #c4121f;
  }

  .features-title,
  .premium-title {
    font-size: 1.5rem;
    font-weight: bold;
    margin: 0 0 1rem 0;
    color: #1e293b;
  }

  .premium-title {
    color: #b91c1c;
  }

  .features-list,
  .premium-list {
    list-style: none;
    padding: 0;
    margin: 0;
  }

  .features-list li,
  .premium-list li {
    font-size: 1.1rem;
    padding: 0.75rem 0;
    border-bottom: 1px dashed rgba(102, 155, 188, 0.3);
    font-family: monospace;
  }

  .premium-list li {
    border-bottom-color: rgba(193, 18, 31, 0.2);
  }

  .features-list li:last-child,
  .premium-list li:last-child {
    border-bottom: none;
  }

  .resumes-section {
    margin-bottom: 3rem;
  }

  .section-title {
    font-size: 2rem;
    font-weight: bold;
    margin: 0 0 1.5rem 0;
    color: #000;
  }

  .resumes-header {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    padding: 1rem 0.5rem;
    border-bottom: 2px solid #94a3b8;
    font-weight: bold;
    font-size: 1.25rem;
    color: #334155;
    background: #f1f5f9;
    border-radius: 8px 8px 0 0;
  }

  .resume-actions-header,
  .resume-status-header,
  .resume-title-header {
    font-size: 1.5rem;
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  /* 1300px */
  @media (max-width: 1300px) {
    .page-title {
      font-size: 2.2rem;
    }
    
    .welcome-message {
      font-size: 1.5rem;
    }
    
    .user-info-section {
      padding: 1.5rem;
    }
    
    .info-label {
      font-size: 1.75rem;
    }
    
    .info-value {
      font-size: 1.35rem;
    }
    
    .section-title {
      font-size: 1.8rem;
    }
    
    .create-resume-btn {
      font-size: 1.5rem;
      padding: 0.4rem 1.2rem;
    }
  }

  /* 1080px */
  @media (max-width: 1080px) {
    .page-title {
      font-size: 2rem;
    }
    
    .welcome-message {
      font-size: 1.3rem;
    }
    
    .info-label {
      min-width: 90px;
    }
    
    .info-value {
      font-size: 1rem;
      padding: 0.4rem 0.8rem;
    }
    
    .action-link {
      font-size: 1rem;
      padding: 0.4rem 0.8rem;
    }
    
    .status-badge {
      font-size: 0.9rem;
      padding: 0.25rem 0.8rem;
    }
    
    .features-title,
    .premium-title {
      font-size: 1.3rem;
    }
    
    .features-list li,
    .premium-list li {
      font-size: 1rem;
    }
    
    .section-title {
      font-size: 1.6rem;
    }
    
    .resumes-header {
      font-size: 1.1rem;
    }
    
    .create-resume-btn {
      font-size: 1.25rem;
      padding: 0.35rem 1rem;
    }
  }

  /* 900px */
  @media (max-width: 900px) {
    .header-top {
      flex-direction: column;
      gap: 1rem;
      align-items: flex-start;
    }
    
    .page-header {
      text-align: center;
    }
    
    .user-info-section {
      padding: 1.2rem;
    }
    
    .info-row {
      flex-wrap: wrap;
      gap: 0.5rem;
    }
    
    .info-label {
      min-width: 80px;
    }
    
    .features-section,
    .premium-section {
      padding: 1.5rem;
    }
    
    .resumes-header {
      grid-template-columns: 1.5fr 1fr 1fr;
      font-size: 1rem;
      padding: 0.8rem 0.3rem;
    }
    
    .create-resume-btn {
      align-self: flex-start;
    }
  }

  /* 650px */
  @media (max-width: 650px) {
    .page-title {
      font-size: 1.8rem;
    }
    
    .welcome-message {
      font-size: 1.25rem;
    }
    
    .user-info-section {
      padding: 1rem;
      gap: 0.5rem;
    }
    
    .info-row {
      flex-direction: column;
      align-items: flex-start;
      gap: 0.3rem;
    }
    
    .info-label {
      min-width: auto;
    }
    
    .info-value {
      width: 100%;
      box-sizing: border-box;
    }
    
    .action-link {
      width: 100%;
      text-align: center;
    }
    
    .status-wrapper {
      width: 100%;
      box-sizing: border-box;
    }
    
    .status-badge {
      display: block;
      text-align: center;
    }
    
    .features-title,
    .premium-title {
      font-size: 1.2rem;
    }
    
    .features-list li,
    .premium-list li {
      font-size: 0.9rem;
      padding: 0.5rem 0;
    }
    
    .section-title {
      font-size: 1.4rem;
    }
    
    .resumes-header {
      grid-template-columns: 1fr;
      gap: 0.5rem;
      text-align: center;
    }
    
    .resume-title-header,
    .resume-status-header,
    .resume-actions-header {
      font-size: 1.25rem;
    }
    
    .resume-actions-header {
      padding-bottom: 1rem;
    }
    
    .header-top {
      flex-direction: column;
      align-items: stretch;
      gap: 0.75rem;
    }
    
    .create-resume-btn {
      width: 100%;
      text-align: center;
    }
  }

  /* 475px */
  @media (max-width: 475px) {
    .page-title {
      font-size: 1.5rem;
    }
    
    .welcome-message {
      font-size: 1rem;
    }
    
    .user-info-section {
      padding: 0.8rem;
    }
    
    .info-label {
      font-size: 1.25rem;
    }
    
    .info-value {
      font-size: 1.15rem;
    }
    
    .action-link {
      font-size: 1.25rem;
    }
    
    .status-badge {
      font-size: 0.8rem;
    }
    
    .features-section,
    .premium-section {
      padding: 1rem;
    }
    
    .features-title,
    .premium-title {
      font-size: 1.1rem;
    }
    
    .features-list li,
    .premium-list li {
      font-size: 0.8rem;
    }
    
    .section-title {
      font-size: 1.2rem;
    }
    
    .create-resume-btn {
      font-size: 1rem;
      padding: 0.4rem 1rem;
    }
  }
</style>
