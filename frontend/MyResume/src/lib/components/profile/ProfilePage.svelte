<script>
    import { onMount } from 'svelte';
    import { goto } from '$app/navigation';
    import { base } from '$app/paths';
    import { auth } from '$lib/authStore';
    import ResumeRow from './ResumeRow.svelte';
    import toast from 'svelte-french-toast';

    const RESUME_API_BASE = 'http://localhost:5052';
    
    let resumes = [];
    let isLoadingResumes = true;

    // Реактивные переменные для ролей и лимитов
    $: userEmail = $auth.user?.email || 'Гость';
    $: userRole = $auth.user?.role || 'user';
    $: isBaseTier = userRole === 'user' || userRole === 'standard';
    $: isPremium = userRole === 'premium' || userRole === 'admin';
    $: maxResumes = isBaseTier ? 2 : 10;
    $: currentResumeCount = resumes.length;
    $: isLimitReached = currentResumeCount >= maxResumes;

    async function fetchResumes() {
        if (!$auth.user?.email) return;
        
        isLoadingResumes = true;
        try {
            const response = await fetch(`${RESUME_API_BASE}/api/resumes/by-email/${$auth.user.email}`);
            if (response.ok) {
                resumes = await response.json();
            } else {
                console.error('Failed to fetch resumes');
            }
        } catch (error) {
            console.error('Error fetching resumes:', error);
        } finally {
            isLoadingResumes = false;
        }
    }

    onMount(() => {
        fetchResumes();
    });

    function goToCreate() {
        if (isLimitReached) {
            toast.error(`Вы достигли лимита резюме для вашего статуса (${maxResumes})`);
            return;
        }
        goto(`${base}/profile/create`);
    }

    function goToPremium() {
        goto(`${base}/profile/premium/`);
    }

    function goToRestorePassword() {
        goto(`${base}/SignIn/restorePassword`);
    }

    function getRoleName(role) {
        switch(role) {
            case 'admin': return 'АДМИНИСТРАТОР';
            case 'premium': return 'ПРЕМИУМ';
            case 'standard':
            case 'user':
            default: return 'СТАНДАРТ';
        }
    }
</script>

<!-- Заголовок и приветствие -->
<div class="page-header">
    <div class="header-top">
        <h1 class="page-title">Личный кабинет</h1>
    </div>
    <p class="welcome-message">ДОБРО ПОЖАЛОВАТЬ!</p>
</div>

<!-- Блок создания резюме -->
<div class="create-resume-section" class:limit-reached={isLimitReached}>
    <button on:click={goToCreate} class="create-resume-btn" disabled={isLimitReached}>
        {isLimitReached ? 'Лимит достигнут' : 'Создать резюме'}
    </button>
    {#if isLimitReached}
        <p class="limit-message">
            Вы использовали все доступные слоты ({currentResumeCount}/{maxResumes}). 
            {#if isBaseTier}
                <button class="upgrade-link" on:click={goToPremium}>Перейдите на Премиум</button>, чтобы создавать до 10 резюме.
            {/if}
        </p>
    {:else}
        <p class="resume-count-info">Использовано {currentResumeCount} из {maxResumes} доступных мест</p>
    {/if}
</div>

<!-- Информация о пользователе-->
<div class="user-info-section">
    <!-- Логин -->
    <div class="info-row">
        <span class="info-label">Логин:</span>
        <span class="info-value">{userEmail}</span>
    </div>
    
    <!-- Сменить пароль -->
    <div class="info-row">
        <button type="button" on:click={goToRestorePassword} class="action-link change-password">Сменить пароль</button>
    </div>
    
    <!-- Статус -->
    <div class="info-row">
        <span class="info-label">Статус:</span>
        <div class="status-wrapper">
            <span class="status-badge" class:premium-badge={isPremium}>
                {getRoleName(userRole)}
            </span>
        </div>
    </div>
    
    <!-- Повысить статус (показываем только обычным пользователям) -->
    {#if isBaseTier}
        <div class="info-row">
            <button on:click={goToPremium} class="action-link upgrade-status">Повысить статус</button>
        </div>
    {/if}
</div>

<!-- Тариф и возможности по роли -->
{#if isBaseTier}
    <section class="tariff-card tariff-card--standard">
        <h3 class="tariff-card-title">Тариф «Стандарт»</h3>
        <ul class="tariff-list">
            <li><strong>Создание резюме</strong> — не более <strong>2</strong> штук</li>
            <li><strong>Конструктор резюме</strong> — полный доступ</li>
            <li><strong>ИИ-консультант</strong> — недоступен</li>
            <li><strong>Поиск резюме</strong> — недоступен</li>
        </ul>
        <p class="tariff-pitch">
            Вам доступно бесплатное создание до 2 резюме. Перейдите на Premium, чтобы снять лимиты, получить доступ к ИИ-консультанту и поиску резюме.
        </p>
        <button type="button" class="tariff-upgrade-btn" on:click={goToPremium}>Узнать про Premium</button>
    </section>
{:else if userRole === 'premium'}
    <section class="tariff-card tariff-card--premium">
        <h3 class="tariff-card-title">Тариф Premium</h3>
        <ul class="tariff-list">
            <li><strong>Создание резюме</strong> — до <strong>10</strong> штук</li>
            <li><strong>Конструктор резюме</strong> — полный доступ</li>
            <li><strong>ИИ-консультант</strong> — доступен в конструкторе (кнопка «Спросить ИИ»)</li>
            <li><strong>Поиск резюме</strong> — доступен</li>
        </ul>
        <p class="tariff-summary tariff-summary--premium">
            Ваш тариф Premium. Вам доступно до 10 резюме, ИИ-консультант, поиск резюме.
        </p>
    </section>
{:else if userRole === 'admin'}
    <section class="tariff-card tariff-card--admin">
        <h3 class="tariff-card-title">Тариф администратора</h3>
        <p class="tariff-admin-lead">Все возможности Premium, плюс инструменты модерации и аналитики.</p>
        <ul class="tariff-list">
            <li><strong>Создание резюме</strong> — до <strong>10</strong> штук</li>
            <li><strong>Конструктор</strong> и <strong>ИИ-консультант</strong></li>
            <li><strong>Поиск резюме</strong> — полный доступ</li>
            <li><strong>Удаление любых резюме</strong> из результатов поиска (кнопка «Удалить» с подтверждением)</li>
            <li><strong>Управление пользователями</strong> и ролями</li>
            <li><strong>Статистика платформы</strong></li>
        </ul>
        <p class="tariff-summary tariff-summary--admin">
            Вы — администратор. Вам доступны все функции Premium, а также модерация: удаление любых резюме из поиска, управление пользователями и просмотр статистики.
        </p>
        <div class="tariff-admin-actions">
            <button type="button" class="action-link admin-stats-btn" on:click={() => goto(`${base}/profile/admin/stats`)}>Статистика</button>
            <button type="button" class="action-link admin-panel-btn" on:click={() => goto(`${base}/profile/admin`)}>Управление пользователями</button>
        </div>
    </section>
{/if}

<!-- Мои резюме -->
<section class="resumes-section">
    <h2 class="section-title">Мои резюме</h2>
    
    {#if isLoadingResumes}
        <div class="loading-resumes">Загрузка ваших резюме...</div>
    {:else if resumes.length === 0}
        <div class="empty-resumes">У вас пока нет созданных резюме. Нажмите «Создать резюме», чтобы начать!</div>
    {:else}
        <!-- Заголовки таблицы -->
        <div class="resumes-header">
            <span class="resume-title-header">Название резюме</span>
            <span class="resume-status-header">Статус</span>
            <span class="resume-actions-header">Действия</span>
        </div>

        <!-- Строки резюме -->
        {#each resumes as resume}
            <ResumeRow 
                id={resume.id}
                title={resume.name || 'Без названия'} 
                initialStatus={resume.status || 'private'}
            />
        {/each}
    {/if}
</section>

<style>
    /* Базовые стили */
    .page-header {
        margin-bottom: 2.5rem;
    }

    .page-title {
        font-size: 2.5rem;
        font-weight: bold;
        margin: 0;
        color: #1d3557;
    }

    .welcome-message {
        font-size: 1.75rem;
        font-weight: bold;
        color: #457b9d;
        margin: 0.5rem 0 0 0;
    }

    /* Секция создания резюме */
    .create-resume-section {
        margin-bottom: 3rem;
        padding: 2.5rem;
        background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
        border-radius: 20px;
        display: flex;
        flex-direction: column;
        align-items: center;
        box-shadow: 0 4px 15px rgba(37, 99, 235, 0.1);
    }

    .create-resume-section.limit-reached {
        background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%);
        border: 1px solid #fca5a5;
    }

    .create-resume-btn {
        background-color: #c1121f;
        border-radius: 12px;
        padding: 1rem 3.5rem;
        color: white;
        font-size: 1.8rem;
        font-weight: 700;
        cursor: pointer;
        transition: all 0.3s ease;
        border: none;
        box-shadow: 0 8px 15px rgba(193, 18, 31, 0.2);
    }

    .create-resume-btn:disabled {
        background-color: #94a3b8;
        cursor: not-allowed;
        box-shadow: none;
        transform: none;
    }

    .create-resume-btn:hover:not(:disabled) {
        background-color: #a8101a;
        transform: translateY(-2px);
        box-shadow: 0 12px 20px rgba(193, 18, 31, 0.3);
    }

    .resume-count-info {
        margin-top: 1rem;
        font-weight: 600;
        color: #475569;
    }

    .limit-message {
        margin-top: 1rem;
        color: #b91c1c;
        font-weight: 600;
        text-align: center;
    }

    .upgrade-link {
        background: none;
        border: none;
        color: #2563eb;
        text-decoration: underline;
        font-weight: 700;
        cursor: pointer;
        padding: 0;
    }

    .user-info-section {
        margin-bottom: 3rem;
        padding: 2rem;
        background: white;
        border-radius: 20px;
        box-shadow: 0 5px 15px rgba(0, 0, 0, 0.05);
        border: 1px solid #e2e8f0;
    }

    .info-row {
        display: flex;
        align-items: center;
        gap: 1.5rem;
        padding: 1rem 0;
        border-bottom: 1px solid #f1f5f9;
    }

    .info-row:last-child {
        border-bottom: none;
    }

    .info-label {
        min-width: 120px;
        font-size: 1.25rem;
        font-weight: 700;
        color: #1e293b;
    }

    .info-value {
        font-size: 1.25rem;
        color: #475569;
    }

    .status-badge {
        background: #f1f5f9;
        color: #475569;
        padding: 0.4rem 1.2rem;
        border-radius: 999px;
        font-size: 1.1rem;
        font-weight: 800;
    }

    .status-badge.premium-badge {
        background: #fef3c7;
        color: #92400e;
        border: 1px solid #f59e0b;
    }

    .action-link {
        background: none;
        border: 1px solid #e2e8f0;
        color: #1d3557;
        font-weight: 600;
        padding: 0.5rem 1.5rem;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.2s;
    }

    .action-link:hover {
        background: #f8fafc;
        border-color: #1d3557;
    }

    li.admin-action-item {
        list-style: none;
        margin-left: 0;
        padding-left: 0;
    }

    li.admin-action-item--lead {
        margin-top: 1rem;
    }

    li.admin-action-item:not(.admin-action-item--lead) {
        margin-top: 0.65rem;
    }

    li.admin-action-item .action-link {
        width: 100%;
        max-width: 22rem;
    }

    @media (max-width: 480px) {
        li.admin-action-item .action-link {
            max-width: 100%;
        }
    }

    .admin-panel-btn {
        background: #2563eb;
        color: white;
        border-color: #2563eb;
    }

    .admin-panel-btn:hover {
        background: #1d4ed8;
        border-color: #1d4ed8;
        color: white;
    }

    .admin-stats-btn {
        background: #f0fdfa;
        color: #0f766e;
        border-color: #5eead4;
    }

    .admin-stats-btn:hover {
        background: #ccfbf1;
        border-color: #0d9488;
        color: #115e59;
    }

    .features-section, .premium-section, .premium-active-section {
        padding: 2rem;
        border-radius: 16px;
        margin-bottom: 2rem;
    }

    .features-section { background: #f8fafc; border-left: 6px solid #457b9d; }
    .premium-section { background: #fffbeb; border-left: 6px solid #f59e0b; }
    .premium-active-section { background: #f0fdf4; border-left: 6px solid #22c55e; }

    .features-title { font-family: var(--font-heading); color: #1d3557; margin-bottom: 1.5rem; font-size: 2rem; }
    .premium-title { font-family: var(--font-heading); color: #92400e; margin-bottom: 1.5rem; font-size: 2rem; }
    .premium-active-title { font-family: var(--font-heading); color: #166534; margin-bottom: 1.25rem; font-size: 2.2rem; }

    .features-list, .premium-list { 
        list-style: disc; 
        padding-left: 1.5rem; 
        margin-bottom: 1rem;
    }
    .features-list li, .premium-list li {
        padding: 0.3rem 0;
        color: #475569;
    }

    .tariff-card {
        padding: 2rem 2rem 1.75rem;
        border-radius: 18px;
        margin-bottom: 2rem;
        border: 1px solid #e2e8f0;
        box-shadow: 0 8px 32px rgba(29, 53, 87, 0.07);
    }

    .tariff-card--standard {
        background: linear-gradient(145deg, #f8fafc 0%, #f0f9ff 100%);
        border-left: 6px solid #457b9d;
    }

    .tariff-card--premium {
        background: linear-gradient(145deg, #f0fdf4 0%, #ecfeff 100%);
        border-left: 6px solid #16a34a;
    }

    .tariff-card--admin {
        background: linear-gradient(145deg, #fffbeb 0%, #fff7ed 100%);
        border-left: 6px solid #ea580c;
    }

    .tariff-card-title {
        font-family: var(--font-heading);
        color: #1d3557;
        margin: 0 0 1rem;
        font-size: 1.75rem;
        font-weight: 800;
        letter-spacing: -0.02em;
    }

    .tariff-admin-lead {
        margin: 0 0 1rem;
        color: #475569;
        font-size: 1rem;
        line-height: 1.5;
    }

    .tariff-list {
        list-style: none;
        padding: 0;
        margin: 0 0 1.25rem;
    }

    .tariff-list li {
        position: relative;
        padding: 0.45rem 0 0.45rem 1.35rem;
        color: #334155;
        line-height: 1.45;
        border-bottom: 1px solid rgba(226, 232, 240, 0.8);
    }

    .tariff-list li:last-child {
        border-bottom: none;
    }

    .tariff-list li::before {
        content: '';
        position: absolute;
        left: 0;
        top: 0.85rem;
        width: 6px;
        height: 6px;
        border-radius: 50%;
        background: linear-gradient(135deg, #457b9d, #669bbc);
    }

    .tariff-pitch {
        margin: 0 0 1.25rem;
        padding: 1rem 1.1rem;
        background: rgba(255, 255, 255, 0.75);
        border-radius: 12px;
        border: 1px solid #bfdbfe;
        color: #1e3a5f;
        font-size: 1.05rem;
        line-height: 1.55;
        font-weight: 500;
    }

    .tariff-upgrade-btn {
        font-family: var(--font-heading);
        font-weight: 700;
        font-size: 0.95rem;
        padding: 0.65rem 1.35rem;
        border-radius: 999px;
        border: none;
        cursor: pointer;
        color: #fff;
        background: linear-gradient(135deg, #2563eb, #1d4ed8);
        box-shadow: 0 4px 14px rgba(37, 99, 235, 0.3);
        transition: transform 0.15s ease, box-shadow 0.2s ease;
    }

    .tariff-upgrade-btn:hover {
        transform: translateY(-1px);
        box-shadow: 0 6px 18px rgba(37, 99, 235, 0.35);
    }

    .tariff-summary {
        margin: 0;
        padding: 0.9rem 1rem;
        border-radius: 12px;
        font-size: 1rem;
        line-height: 1.5;
        font-weight: 600;
    }

    .tariff-summary--premium {
        background: rgba(255, 255, 255, 0.85);
        border: 1px solid #bbf7d0;
        color: #14532d;
    }

    .tariff-summary--admin {
        background: rgba(255, 255, 255, 0.9);
        border: 1px solid #fed7aa;
        color: #7c2d12;
    }

    .tariff-admin-actions {
        display: flex;
        flex-wrap: wrap;
        gap: 0.75rem;
        margin-top: 1.25rem;
    }

    .tariff-admin-actions .action-link {
        width: auto;
        max-width: none;
    }

    .loading-resumes, .empty-resumes {
        text-align: center;
        padding: 3rem;
        font-size: 1.2rem;
        color: #64748b;
        background: #f8fafc;
        border-radius: 12px;
    }

    .resumes-header {
        display: grid;
        grid-template-columns: 2fr 1fr 1fr;
        padding: 1rem;
        background: #1d3557;
        color: white;
        border-radius: 8px 8px 0 0;
        font-weight: 700;
    }

    @media (max-width: 768px) {
        .info-row { flex-direction: column; align-items: flex-start; gap: 0.5rem; }
        .resumes-header { display: none; }
    }

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
    font-family: var(--font-heading);
    font-size: 3rem;
    font-weight: 800;
    margin: 0;
    color: #000;
    letter-spacing: -0.04em;
  }

  .welcome-message {
    font-size: 1.75rem;
    font-weight: 500;
    color: rgba(0, 0, 0, 0.6);
    margin: 0;
    letter-spacing: 0.1em;
    text-transform: uppercase;
  }

  /* Секция создания резюме */
  .create-resume-section {
    margin-bottom: 3rem;
    padding: 2.5rem;
    background: linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%);
    border-radius: 20px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    box-shadow: 0 4px 15px rgba(37, 99, 235, 0.1);
  }

  .create-resume-btn {
    background-color: rgba(193, 18, 31, 0.85);
    border-radius: 12px;
    padding: 1rem 3rem;
    color: white;
    font-size: 2rem;
    font-weight: 700;
    cursor: pointer;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    border: none;
    box-shadow: 0 8px 20px rgba(193, 18, 31, 0.3);
    width: 100%;
    max-width: 400px;
  }

  .create-resume-btn:hover {
    background-color: #c4121f;
    transform: translateY(-3px) scale(1.02);
    box-shadow: 0 12px 25px rgba(193, 18, 31, 0.4);
  }

  .create-resume-hint {
    margin: 1rem 0 0 0;
    font-size: 1.1rem;
    color: #475569;
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
    
    .create-resume-section {
      padding: 2rem;
    }
    
    .create-resume-btn {
      font-size: 1.8rem;
      padding: 0.8rem 2.5rem;
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
      font-size: 1.5rem;
      padding: 0.75rem 2rem;
    }
    
    .create-resume-hint {
      font-size: 1rem;
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
      padding: 0.3rem 0;
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
    
    .create-resume-section {
      padding: 1.5rem;
    }
    
    .create-resume-btn {
      font-size: 1.35rem;
      padding: 0.7rem 1.5rem;
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
      font-size: 1.2rem;
      padding: 0.6rem 1rem;
      width: 100%;
    }
    
    .create-resume-hint {
      font-size: 0.9rem;
    }
  }
</style>
