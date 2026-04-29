<script>
  import { onMount } from 'svelte';
  import { auth } from '$lib/authStore';
  import { authApi } from '$lib/authApi';
  import { goto } from '$app/navigation';
  import { base } from '$app/paths';
  import toast from 'svelte-french-toast';
  import { beginRemoteLoad, endRemoteLoad } from '$lib/stores/remoteLoading.js';

  const RESUME_API_BASE = 'http://localhost:5052';

  let loading = true;
  /** @type {{ totalUsers: number, usersByRole: { user: number, premium: number, admin: number }, usersByStatus: { active: number, pending: number } } | null} */
  let accountStats = null;
  let resumeCount = null;
  let articleCount = null;

  onMount(() => {
    if (!$auth.isAuthenticated || $auth.user?.role !== 'admin') {
      goto(`${base}/`);
      return;
    }
    loadAll();
  });

  /** Считаем сводку из списка пользователей — тот же источник, что и админ-таблица (`GET /admin/users`). */
  function buildAccountStatsFromUsers(users) {
    if (!Array.isArray(users)) return null;
    const usersByRole = { user: 0, premium: 0, admin: 0 };
    const usersByStatus = { active: 0, pending: 0 };
    for (const u of users) {
      const r = u.role;
      if (r === 'user' || r === 'premium' || r === 'admin') usersByRole[r]++;
      const s = u.status;
      if (s === 'active' || s === 'pending') usersByStatus[s]++;
    }
    return {
      totalUsers: users.length,
      usersByRole,
      usersByStatus
    };
  }

  async function loadAll() {
    loading = true;
    beginRemoteLoad();
    try {
      const [usersRes, resumesRes, articlesRes] = await Promise.all([
        authApi.get('/admin/users'),
        fetch(`${RESUME_API_BASE}/api/resumes/basic`),
        fetch(`${RESUME_API_BASE}/api/articles/preview`)
      ]);

      if (usersRes.success && Array.isArray(usersRes.users)) {
        accountStats = buildAccountStatsFromUsers(usersRes.users);
      } else {
        toast.error('Не удалось загрузить статистику аккаунтов');
      }

      if (resumesRes.ok) {
        const data = await resumesRes.json();
        resumeCount = Array.isArray(data) ? data.length : 0;
      } else {
        resumeCount = null;
      }

      if (articlesRes.ok) {
        const data = await articlesRes.json();
        articleCount = Array.isArray(data) ? data.length : 0;
      } else {
        articleCount = null;
      }
    } catch (e) {
      console.error(e);
      toast.error('Ошибка загрузки данных');
    } finally {
      loading = false;
      endRemoteLoad();
    }
  }
</script>

<svelte:head>
  <title>Статистика | MyResume</title>
</svelte:head>

<div class="stats-page">
  <div class="stats-shell">
    <header class="stats-header">
      <button type="button" class="back-btn" on:click={() => goto(`${base}/profile`)}>
        ← К профилю
      </button>
      <div class="header-text">
        <p class="stats-eyebrow">Только для администраторов</p>
        <h1 class="stats-title">Статистика платформы</h1>
        <p class="stats-sub">Сводка по учётным записям и контенту</p>
      </div>
      <button type="button" class="refresh-btn" on:click={loadAll} disabled={loading}>
        Обновить
      </button>
    </header>

    {#if loading}
      <div class="loading-block">
        <div class="spinner" />
        <p>Сбор данных…</p>
      </div>
    {:else if accountStats}
      <section class="grid hero-grid" aria-label="Основные показатели">
        <article class="card card-accent">
          <span class="card-label">Пользователей</span>
          <span class="card-value">{accountStats.totalUsers}</span>
          <span class="card-hint">всего в базе авторизации</span>
        </article>
        <article class="card">
          <span class="card-label">Резюме</span>
          <span class="card-value">{resumeCount ?? '—'}</span>
          <span class="card-hint">записей в MariaDB</span>
        </article>
        <article class="card">
          <span class="card-label">Статей</span>
          <span class="card-value">{articleCount ?? '—'}</span>
          <span class="card-hint">в разделе материалов</span>
        </article>
      </section>

      <section class="section-block" aria-label="По ролям">
        <h2 class="section-title">Распределение по ролям</h2>
        <div class="grid roles-grid">
          <article class="card card-role card-user">
            <span class="card-label">Стандарт</span>
            <span class="card-value">{accountStats.usersByRole.user}</span>
          </article>
          <article class="card card-role card-premium">
            <span class="card-label">Премиум</span>
            <span class="card-value">{accountStats.usersByRole.premium}</span>
          </article>
          <article class="card card-role card-admin">
            <span class="card-label">Админы</span>
            <span class="card-value">{accountStats.usersByRole.admin}</span>
          </article>
        </div>
      </section>

      <section class="section-block" aria-label="По статусу аккаунта">
        <h2 class="section-title">Статус регистрации</h2>
        <div class="grid status-grid">
          <article class="card card-status">
            <span class="card-label">Активные</span>
            <span class="card-value">{accountStats.usersByStatus.active}</span>
          </article>
          <article class="card card-status card-pending">
            <span class="card-label">Ожидают подтверждения</span>
            <span class="card-value">{accountStats.usersByStatus.pending}</span>
          </article>
        </div>
      </section>
    {/if}
  </div>
</div>

<style>
  .stats-page {
    min-height: 60vh;
    padding: 1rem 0 3rem;
  }

  .stats-shell {
    max-width: 960px;
    margin: 0 auto;
    padding: 0 clamp(0.5rem, 3vw, 1rem);
    width: 100%;
  }

  .stats-header {
    display: flex;
    flex-wrap: wrap;
    align-items: flex-start;
    gap: 1rem 1.5rem;
    margin-bottom: 2.25rem;
    padding-bottom: 1.5rem;
    border-bottom: 1px solid rgba(29, 53, 87, 0.12);
  }

  @media (max-width: 600px) {
    .stats-header {
      flex-direction: column;
      align-items: stretch;
    }

    .stats-header .back-btn,
    .stats-header .refresh-btn {
      align-self: flex-start;
    }
  }

  .header-text {
    flex: 1;
    min-width: 200px;
  }

  .stats-eyebrow {
    margin: 0 0 0.35rem;
    font-size: 0.75rem;
    font-weight: 700;
    letter-spacing: 0.08em;
    text-transform: uppercase;
    color: #457b9d;
  }

  .stats-title {
    margin: 0;
    font-size: clamp(1.65rem, 4vw, 2.1rem);
    font-weight: 800;
    color: #1d3557;
    letter-spacing: -0.02em;
  }

  .stats-sub {
    margin: 0.5rem 0 0;
    font-size: 0.95rem;
    color: #64748b;
    max-width: 36rem;
    line-height: 1.5;
  }

  .back-btn,
  .refresh-btn {
    font-family: inherit;
    font-weight: 600;
    border-radius: 10px;
    cursor: pointer;
    transition: background 0.2s, color 0.2s, opacity 0.2s;
  }

  .back-btn {
    background: #f1f5f9;
    border: none;
    padding: 0.55rem 1rem;
    color: #1d3557;
    align-self: center;
  }

  .back-btn:hover {
    background: #e2e8f0;
  }

  .refresh-btn {
    background: #1d3557;
    color: #fff;
    border: none;
    padding: 0.55rem 1.15rem;
    align-self: center;
  }

  .refresh-btn:hover:not(:disabled) {
    background: #457b9d;
  }

  .refresh-btn:disabled {
    opacity: 0.55;
    cursor: not-allowed;
  }

  .loading-block {
    text-align: center;
    padding: 4rem 1rem;
    color: #64748b;
  }

  .spinner {
    width: 44px;
    height: 44px;
    margin: 0 auto 1rem;
    border: 3px solid #e2e8f0;
    border-top-color: #457b9d;
    border-radius: 50%;
    animation: spin 0.85s linear infinite;
  }

  @keyframes spin {
    to {
      transform: rotate(360deg);
    }
  }

  .grid {
    display: grid;
    gap: 1rem;
  }

  .hero-grid {
    grid-template-columns: repeat(3, 1fr);
    margin-bottom: 2.5rem;
  }

  @media (max-width: 720px) {
    .hero-grid {
      grid-template-columns: 1fr;
    }
  }

  .section-block {
    margin-bottom: 2.25rem;
  }

  .section-title {
    margin: 0 0 1rem;
    font-size: 1.1rem;
    font-weight: 700;
    color: #1d3557;
  }

  .roles-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  @media (max-width: 640px) {
    .roles-grid {
      grid-template-columns: 1fr;
    }
  }

  .status-grid {
    grid-template-columns: repeat(2, 1fr);
    max-width: 560px;
  }

  @media (max-width: 480px) {
    .status-grid {
      grid-template-columns: 1fr;
      max-width: none;
    }
  }

  .card {
    position: relative;
    background: #fff;
    border-radius: 16px;
    padding: 1.35rem 1.5rem;
    box-shadow: 0 4px 24px rgba(29, 53, 87, 0.08);
    border: 1px solid rgba(102, 155, 188, 0.25);
    display: flex;
    flex-direction: column;
    gap: 0.35rem;
    overflow: hidden;
  }

  .card::before {
    content: '';
    position: absolute;
    inset: 0 0 auto 0;
    height: 3px;
    background: linear-gradient(90deg, #669bbc, #a8dadc);
    opacity: 0.85;
  }

  .card-accent::before {
    background: linear-gradient(90deg, #1d3557, #457b9d);
  }

  .card-label {
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    color: #64748b;
  }

  .card-value {
    font-size: clamp(1.75rem, 4vw, 2.35rem);
    font-weight: 800;
    color: #1d3557;
    line-height: 1.1;
  }

  .card-hint {
    font-size: 0.85rem;
    color: #94a3b8;
    margin-top: 0.15rem;
  }

  .card-role .card-value {
    font-size: 1.85rem;
  }

  .card-user::before {
    background: linear-gradient(90deg, #e2e8f0, #94a3b8);
  }

  .card-premium::before {
    background: linear-gradient(90deg, #fbbf24, #d97706);
  }

  .card-admin::before {
    background: linear-gradient(90deg, #f87171, #b91c1c);
  }

  .card-pending::before {
    background: linear-gradient(90deg, #cbd5e1, #64748b);
  }
</style>
