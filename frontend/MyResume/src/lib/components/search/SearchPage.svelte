<script>
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { base } from '$app/paths';
  import { auth } from '$lib/authStore';
  import toast from 'svelte-french-toast';
  import {
    loadTagsByCategories,
    categoryDisplayNames
  } from '$lib/tagCategories.js';

  let loading = false;
  let searchResults = [];
  let searchPerformed = false;

  let tagsByCategory = {};
  /** Порядок секций фильтров: IT, затем остальные по алфавиту (только непустые категории) */
  let categoryOrder = [];
  let allTags = [];
  let selectedTagIds = [];

  const API_BASE_URL = 'http://localhost:5052';

  async function loadTags() {
    try {
      const { tagsByCategory: byCat, categoryOrder: order, allTags: tags } =
        await loadTagsByCategories(API_BASE_URL);
      tagsByCategory = { ...byCat };
      categoryOrder = [...order];
      allTags = [...tags];
    } catch (err) {
      console.error('Ошибка загрузки тегов:', err);
    }
  }

  // Поиск резюме по выбранным тегам
  async function searchResumes() {
    if (selectedTagIds.length === 0) {
      // Если теги не выбраны, показываем все публичные резюме
      await loadAllPublicResumes();
      return;
    }

    loading = true;
    searchPerformed = true;

    try {
      const tagIdsParam = selectedTagIds.join(',');
      const response = await fetch(`${API_BASE_URL}/api/resumes/public?tagIds=${tagIdsParam}`);

      if (response.ok) {
        searchResults = await response.json();
      } else {
        searchResults = [];
      }

    } catch (err) {
      console.error('Ошибка поиска:', err);
      searchResults = [];
    } finally {
      loading = false;
    }
  }

  // Загрузка всех публичных резюме
  async function loadAllPublicResumes() {
    loading = true;
    searchPerformed = true;

    try {
      const response = await fetch(`${API_BASE_URL}/api/resumes/public`);

      if (response.ok) {
        searchResults = await response.json();
      } else {
        searchResults = [];
      }

    } catch (err) {
      console.error('Ошибка загрузки:', err);
      searchResults = [];
    } finally {
      loading = false;
    }
  }

  // Переключение выбора тега
  function toggleTag(tagId) {
    if (selectedTagIds.includes(tagId)) {
      selectedTagIds = selectedTagIds.filter(id => id !== tagId);
    } else {
      selectedTagIds = [...selectedTagIds, tagId];
    }

    // Автоматически выполняем поиск при изменении фильтра
    searchResumes();
  }

  // Сброс всех фильтров
  function resetFilters() {
    selectedTagIds = [];
    searchResumes();
  }

  // Скачивание PDF
  async function downloadResume(resumeId, filename) {
    try {
      const response = await fetch(`${API_BASE_URL}/api/resumes/${resumeId}/download`);

      if (response.ok) {
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename || `resume_${resumeId}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      } else {
        console.error('Ошибка скачивания');
      }
    } catch (err) {
      console.error('Ошибка:', err);
    }
  }

  // Удаление резюме (только для Админов)
  async function deleteResume(id) {
    if (!confirm('Вы уверены, что хотите удалить это резюме? Оно будет навсегда удалено из базы данных.')) {
      return;
    }

    try {
      const userEmail = $auth.user?.email || '';
      const userRole = $auth.user?.role || 'user';
      
      const response = await fetch(`${API_BASE_URL}/api/resumes/${id}?email=${encodeURIComponent(userEmail)}&role=${userRole}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        toast.success('Резюме успешно удалено');
        searchResults = searchResults.filter(r => r.id !== id);
      } else {
        const data = await response.json();
        throw new Error(data.message || 'Ошибка при удалении');
      }
    } catch (err) {
      toast.error(err.message);
    }
  }

  onMount(async () => {
    // Редирект для обычных пользователей, которым запрещен поиск по Use-Case
    if ($auth.user?.role === 'user' || $auth.user?.role === 'standard') {
      toast.error('Доступ к поиску резюме ограничен. Повысьте статус до Премиум.');
      goto(`${base}/profile`);
      return;
    }
    await loadTags();
    await loadAllPublicResumes();
  });
</script>

<div class="search-page">
  <!-- Заголовок -->
  <div class="search-header">
    <h1 class="search-title">НАЙДИТЕ ИДЕАЛЬНОЕ РЕЗЮМЕ</h1>
    <p class="search-subtitle">ВЫБЕРИТЕ НАВЫКИ ДЛЯ ПОИСКА</p>
  </div>

  <!-- Фильтры по категориям тегов -->
  <div class="categories-section">
    <div class="filters-header">
      <div class="filters-title-wrap">
        <h3 class="filters-title">Фильтры по навыкам</h3>
        <p class="filters-title-sub">Уточните профиль кандидата по стеку и компетенциям</p>
      </div>
      {#if selectedTagIds.length > 0}
        <button type="button" class="reset-filters-btn" on:click={resetFilters}>Сбросить всё</button>
      {/if}
    </div>

    <p class="filters-hint">
      <span class="filters-hint-mark" aria-hidden="true"></span>
      <span class="filters-hint-text">Откройте категорию — выбранные навыки сразу участвуют в поиске.</span>
    </p>

    <div class="filters-accordion">
      {#each categoryOrder as category, i}
        {#if tagsByCategory[category] && tagsByCategory[category].length > 0}
          <details class="filter-category-panel" open={i === 0}>
            <summary class="filter-category-summary">
              <span class="filter-category-title">{categoryDisplayNames[category] || category}</span>
              <span class="filter-category-meta">
                <span class="filter-count-badge">{tagsByCategory[category].length}</span>
                <span class="filter-chevron" aria-hidden="true"><span class="filter-chevron-inner"></span></span>
              </span>
            </summary>
            <div class="filter-panel-body">
              <div class="filter-tags filter-tags--grid">
                {#each tagsByCategory[category] as tag}
                  <button
                    type="button"
                    class="filter-tag filter-tag--compact {selectedTagIds.includes(tag.id) ? 'active' : ''}"
                    on:click={() => toggleTag(tag.id)}
                  >
                    {tag.name}
                  </button>
                {/each}
              </div>
            </div>
          </details>
        {/if}
      {/each}
    </div>
  </div>

  <!-- Результаты поиска -->
  <section class="results-section">
    <h2 class="section-title">
      {#if searchPerformed}
        Результаты поиска ({searchResults.length})
      {:else}
        Топ резюме
      {/if}
    </h2>

    {#if loading}
      <div class="loading-state">
        <div class="spinner"></div>
        <p>Загрузка резюме...</p>
      </div>
    {:else if searchResults.length === 0 && searchPerformed}
      <div class="empty-state">
        <p>По вашему запросу ничего не найдено</p>
        <p class="empty-hint">Попробуйте выбрать другие навыки</p>
      </div>
    {:else}
      <div class="resumes-grid">
        {#each searchResults as resume}
          <div class="resume-card">
            <div class="card-content">
              <h3 class="card-title">{resume.name || 'Резюме'}</h3>
              <div class="card-email">{resume.email}</div>
              {#if resume.tags}
                <div class="card-tags">
                  {#each resume.tags.split(', ') as tag}
                    <span class="tag">{tag}</span>
                  {/each}
                </div>
              {/if}
            </div>
            <div class="actions-group">
              <button
                      class="download-btn"
                      on:click={() => downloadResume(resume.id, resume.name)}
              >
                <span class="download-icon">⬇️</span>
                <span class="download-text">Скачать PDF</span>
              </button>

              {#if $auth.user && $auth.user.role === 'admin'}
                <button
                        class="delete-btn"
                        on:click={() => deleteResume(resume.id)}
                >
                  Удалить
                </button>
              {/if}
            </div>
          </div>
        {/each}
      </div>
    {/if}
  </section>
</div>

<style>
  .search-page {
    max-width: 1200px;
    width: 100%;
    margin: 0 auto;
    padding: clamp(1rem, 3vw, 2rem);
    color: #333;
    box-sizing: border-box;
    overflow-x: hidden;
  }

  /* Заголовок */
  .search-header {
    margin-bottom: 2.5rem;
  }

  .search-title {
    font-family: var(--font-heading);
    font-size: 2.85rem;
    font-weight: 800;
    margin: 0 0 0.5rem 0;
    color: #000;
    letter-spacing: -0.03em;
  }

  .search-subtitle {
    font-size: 1.35rem;
    color: #475569;
    font-weight: 500;
    margin: 0;
  }

  /* Фильтры */
  .categories-section {
    margin-bottom: 2rem;
    padding: clamp(1.15rem, 3.2vw, 1.65rem);
    background: linear-gradient(165deg, #ffffff 0%, #f8fafc 42%, #f0f9ff 100%);
    border-radius: 20px;
    border: 1px solid rgba(102, 155, 188, 0.38);
    box-shadow:
      0 10px 40px rgba(29, 53, 87, 0.08),
      0 1px 0 rgba(255, 255, 255, 0.75) inset;
  }

  .filters-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 0.85rem;
    flex-wrap: wrap;
    gap: 1rem;
  }

  .filters-title-wrap {
    flex: 1;
    min-width: 0;
  }

  .filters-title {
    font-family: var(--font-heading);
    font-size: clamp(1.15rem, 2.8vw, 1.35rem);
    font-weight: 800;
    color: #1d3557;
    margin: 0 0 0.25rem;
    letter-spacing: -0.03em;
    position: relative;
    display: inline-block;
  }

  .filters-title::after {
    content: '';
    position: absolute;
    left: 0;
    bottom: -0.2rem;
    width: 2.75rem;
    height: 3px;
    border-radius: 999px;
    background: linear-gradient(90deg, #457b9d, #669bbc);
  }

  .filters-title-sub {
    margin: 0.55rem 0 0;
    font-size: 0.82rem;
    color: #64748b;
    line-height: 1.4;
    max-width: 28rem;
  }

  .reset-filters-btn {
    flex-shrink: 0;
    font-family: var(--font-heading);
    font-size: 0.78rem;
    font-weight: 700;
    letter-spacing: 0.04em;
    text-transform: uppercase;
    color: #b91c1c;
    background: linear-gradient(180deg, #fff 0%, #fef2f2 100%);
    border: 1px solid #fecaca;
    cursor: pointer;
    padding: 0.45rem 0.9rem;
    border-radius: 999px;
    transition: transform 0.15s ease, box-shadow 0.2s ease, border-color 0.2s ease;
    box-shadow: 0 1px 3px rgba(185, 28, 28, 0.08);
  }

  .reset-filters-btn:hover {
    border-color: #f87171;
    box-shadow: 0 4px 14px rgba(185, 28, 28, 0.12);
    transform: translateY(-1px);
  }

  .filters-hint {
    display: flex;
    align-items: flex-start;
    gap: 0.55rem;
    margin: 0 0 1rem;
    font-size: 0.8rem;
    color: #475569;
    line-height: 1.45;
  }

  .filters-hint-mark {
    flex-shrink: 0;
    width: 8px;
    height: 8px;
    margin-top: 0.32em;
    border-radius: 50%;
    background: linear-gradient(135deg, #457b9d, #669bbc);
    box-shadow: 0 0 0 4px rgba(102, 155, 188, 0.2);
  }

  .filters-hint-text {
    flex: 1;
    min-width: 0;
  }

  .filters-accordion {
    display: flex;
    flex-direction: column;
    gap: 0.55rem;
  }

  .filter-category-panel {
    border-radius: 14px;
    background: #fff;
    overflow: hidden;
    border: 1px solid #e8eef4;
    box-shadow: 0 2px 10px rgba(15, 23, 42, 0.05);
    transition: box-shadow 0.25s ease, border-color 0.25s ease;
  }

  .filter-category-panel[open] {
    border-color: rgba(69, 123, 157, 0.45);
    box-shadow:
      0 8px 28px rgba(29, 53, 87, 0.1),
      0 0 0 1px rgba(69, 123, 157, 0.1);
  }

  .filter-category-summary {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 0.65rem;
    padding: 0.65rem 1rem 0.65rem 0.85rem;
    cursor: pointer;
    list-style: none;
    font-family: var(--font-heading);
    font-weight: 700;
    font-size: 0.9rem;
    color: #1d3557;
    user-select: none;
    background: linear-gradient(90deg, #f8fafc 0%, #f1f5f9 100%);
    border-left: 3px solid transparent;
    transition: background 0.2s ease, border-color 0.2s ease;
  }

  .filter-category-summary::-webkit-details-marker {
    display: none;
  }

  .filter-category-summary:focus-visible {
    outline: 2px solid #457b9d;
    outline-offset: 2px;
    z-index: 1;
  }

  .filter-category-summary:hover {
    background: linear-gradient(90deg, #f0f9ff 0%, #f8fafc 100%);
  }

  .filter-category-panel[open] .filter-category-summary {
    background: linear-gradient(90deg, #eff6ff 0%, #f8fafc 100%);
    border-left-color: #457b9d;
  }

  .filter-category-title {
    flex: 1;
    min-width: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    letter-spacing: -0.02em;
  }

  .filter-category-meta {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    flex-shrink: 0;
  }

  .filter-count-badge {
    font-size: 0.68rem;
    font-weight: 800;
    letter-spacing: 0.02em;
    color: #1d3557;
    background: linear-gradient(135deg, #e0f2fe 0%, #dbeafe 100%);
    padding: 0.18rem 0.48rem;
    border-radius: 999px;
    border: 1px solid rgba(69, 123, 157, 0.25);
  }

  .filter-chevron {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 1.65rem;
    height: 1.65rem;
    border-radius: 50%;
    background: rgba(255, 255, 255, 0.9);
    border: 1px solid #e2e8f0;
    transition: background 0.2s ease, border-color 0.2s ease;
  }

  .filter-chevron-inner {
    display: block;
    width: 0.36rem;
    height: 0.36rem;
    border-right: 2px solid #457b9d;
    border-bottom: 2px solid #457b9d;
    transform: rotate(-45deg);
    margin-top: -0.18rem;
    transition: transform 0.25s ease, border-color 0.2s ease;
  }

  .filter-category-panel[open] .filter-chevron {
    background: linear-gradient(135deg, #457b9d, #1d3557);
    border-color: transparent;
  }

  .filter-category-panel[open] .filter-chevron-inner {
    border-color: #fff;
    transform: rotate(135deg);
    margin-top: 0.1rem;
  }

  .filter-panel-body {
    padding: 0.65rem 0.75rem 0.75rem;
    background: linear-gradient(180deg, #fafbfc 0%, #ffffff 45%);
    border-top: 1px solid rgba(226, 232, 240, 0.95);
    max-height: 11.5rem;
    overflow-y: auto;
    -webkit-overflow-scrolling: touch;
    scrollbar-width: thin;
    scrollbar-color: rgba(69, 123, 157, 0.45) #f1f5f9;
  }

  .filter-panel-body::-webkit-scrollbar {
    width: 6px;
  }

  .filter-panel-body::-webkit-scrollbar-thumb {
    background: linear-gradient(180deg, #94a3b8, #64748b);
    border-radius: 999px;
  }

  .filter-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.35rem;
  }

  .filter-tags--grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(7.25rem, 1fr));
    gap: 0.4rem;
  }

  @media (min-width: 640px) {
    .filter-tags--grid {
      grid-template-columns: repeat(auto-fill, minmax(8rem, 1fr));
    }
  }

  .filter-tag {
    font-family: var(--font-body);
    background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
    border: 1px solid #e2e8f0;
    color: #334155;
    padding: 0.5rem 1.25rem;
    border-radius: 999px;
    font-size: 0.95rem;
    font-weight: 600;
    cursor: pointer;
    transition:
      background 0.2s ease,
      border-color 0.2s ease,
      color 0.2s ease,
      box-shadow 0.2s ease,
      transform 0.15s ease;
    box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
  }

  .filter-tag--compact {
    padding: 0.32rem 0.5rem;
    border-radius: 999px;
    font-size: 0.72rem;
    line-height: 1.3;
    text-align: center;
    word-break: break-word;
  }

  .filter-tag:hover {
    border-color: #94a3b8;
    box-shadow: 0 2px 10px rgba(29, 53, 87, 0.08);
    transform: translateY(-1px);
  }

  .filter-tag--compact:hover {
    background: linear-gradient(180deg, #f8fafc 0%, #f1f5f9 100%);
  }

  .filter-tag.active {
    background: linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%);
    border-color: transparent;
    color: #fff;
    box-shadow: 0 3px 14px rgba(37, 99, 235, 0.35);
  }

  .filter-tag--compact.active {
    border-color: rgba(255, 255, 255, 0.35);
  }

  /* Результаты */
  .results-section {
    margin-bottom: 3rem;
  }

  .section-title {
    font-size: 2rem;
    font-weight: bold;
    margin: 0 0 1.5rem 0;
    color: #000;
  }

  .resumes-grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 2rem;
  }

  .resume-card {
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 20px;
    padding: 1.5rem;
    border: 1px solid rgba(102, 155, 188, 0.2);
    transition: all 0.2s;
    display: flex;
    flex-direction: column;
    gap: 1.5rem;
  }

  .resume-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(102, 155, 188, 0.15);
  }

  .card-content {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
  }

  .card-title {
    font-size: 1.3rem;
    font-weight: bold;
    margin: 0;
    color: #1e293b;
    border-bottom: 2px solid rgba(102, 155, 188, 0.2);
    padding-bottom: 0.75rem;
  }

  .card-email {
    font-size: 0.85rem;
    color: #64748b;
    word-break: break-all;
  }

  .card-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
  }

  .tag {
    background: #f1f5f9;
    color: #475569;
    padding: 0.25rem 0.75rem;
    border-radius: 999px;
    font-size: 0.75rem;
    font-weight: 500;
  }

  .download-btn {
    background-color: #2563eb;
    color: white;
    border: none;
    border-radius: 8px;
    padding: 0.75rem 1rem;
    font-size: 1rem;
    font-weight: 500;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    transition: all 0.2s;
    width: 100%;
  }

  .download-btn:hover {
    background-color: #1d4ed8;
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(37, 99, 235, 0.3);
  }

  .actions-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    width: 100%;
  }

  .delete-btn {
    background-color: transparent;
    color: #ef4444;
    border: 1px solid #ef4444;
    border-radius: 8px;
    padding: 0.6rem 1rem;
    font-size: 0.9rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
    width: 100%;
  }

  .delete-btn:hover {
    background-color: #fef2f2;
    transform: translateY(-1px);
  }

  /* Состояния */
  .loading-state, .empty-state {
    text-align: center;
    padding: 3rem;
  }

  .spinner {
    width: 48px;
    height: 48px;
    border: 3px solid #e2e8f0;
    border-top-color: #2563eb;
    border-radius: 50%;
    margin: 0 auto 1rem;
    animation: spin 0.8s linear infinite;
  }

  @keyframes spin {
    to { transform: rotate(360deg); }
  }

  .empty-state p {
    color: #64748b;
    font-size: 1.1rem;
  }

  .empty-hint {
    font-size: 0.9rem;
    color: #94a3b8;
    margin-top: 0.5rem;
  }

  /* Адаптивность */
  @media (max-width: 900px) {
    .resumes-grid {
      grid-template-columns: repeat(2, 1fr);
    }
  }

  @media (max-width: 650px) {
    .search-page {
      padding: 0.85rem 0.65rem;
    }

    .search-title {
      font-size: clamp(1.35rem, 6vw, 1.8rem);
    }

    .search-subtitle {
      font-size: 1rem;
    }

    .resumes-grid {
      grid-template-columns: 1fr;
    }

    .filters-header {
      flex-direction: column;
      align-items: flex-start;
    }

    .categories-section {
      padding: 1.25rem;
    }
  }

  @media (max-width: 400px) {
    .search-title {
      font-size: 1.35rem;
    }

    .categories-section {
      padding: 1rem;
      border-radius: 14px;
    }
  }
</style>