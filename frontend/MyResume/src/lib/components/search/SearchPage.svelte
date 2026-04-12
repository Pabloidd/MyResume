<script>
  import { onMount } from 'svelte';
  import { auth } from '$lib/authStore';
  import toast from 'svelte-french-toast';

  let loading = false;
  let searchResults = [];
  let searchPerformed = false;

  // Теги из БД
  let tagsByCategory = {};
  let allTags = [];
  let selectedTagIds = [];

  const API_BASE_URL = 'http://localhost:5052';

  // Категории для отображения (порядок и названия)
  const categoryDisplayNames = {
    'frontend': 'Frontend',
    'backend': 'Backend',
    'database': 'Базы данных'
  };

  // Загрузка тегов из БД
  async function loadTags() {
    try {
      // Загружаем теги по каждой категории
      const categories = ['frontend', 'backend', 'database'];

      for (const category of categories) {
        const response = await fetch(`${API_BASE_URL}/api/tags/category/${category}`);

        if (response.ok) {
          const data = await response.json();
          tagsByCategory[category] = data;
          allTags = [...allTags, ...data];
        }
      }

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
    if ($auth.user?.role === 'user') {
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
      <h3 class="filters-title">Фильтры по навыкам</h3>
      {#if selectedTagIds.length > 0}
        <button class="reset-filters-btn" on:click={resetFilters}>Сбросить все</button>
      {/if}
    </div>

    {#each Object.keys(tagsByCategory) as category}
      {#if tagsByCategory[category] && tagsByCategory[category].length > 0}
        <div class="filter-category">
          <h4 class="category-title">{categoryDisplayNames[category] || category}</h4>
          <div class="filter-tags">
            {#each tagsByCategory[category] as tag}
              <button
                      class="filter-tag {selectedTagIds.includes(tag.id) ? 'active' : ''}"
                      on:click={() => toggleTag(tag.id)}
              >
                {tag.name}
              </button>
            {/each}
          </div>
        </div>
      {/if}
    {/each}
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
    margin: 0 auto;
    padding: 2rem;
    color: #333;
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
    margin-bottom: 3rem;
    padding: 2rem;
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 20px;
    border: 1px solid rgba(102, 155, 188, 0.2);
  }

  .filters-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
    gap: 1rem;
  }

  .filters-title {
    font-size: 1.2rem;
    font-weight: 600;
    color: #1e293b;
    margin: 0;
  }

  .reset-filters-btn {
    background: none;
    border: none;
    color: #ef4444;
    font-size: 0.9rem;
    cursor: pointer;
    padding: 0.25rem 0.75rem;
    border-radius: 6px;
    transition: all 0.2s;
  }

  .reset-filters-btn:hover {
    background: #fee2e2;
  }

  .filter-category {
    margin-bottom: 1.5rem;
  }

  .category-title {
    font-size: 0.9rem;
    font-weight: 600;
    color: #64748b;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    margin: 0 0 0.75rem 0;
  }

  .filter-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.75rem;
  }

  .filter-tag {
    background: #f1f5f9;
    border: 1px solid #e2e8f0;
    color: #475569;
    padding: 0.5rem 1.25rem;
    border-radius: 999px;
    font-size: 0.95rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.2s;
  }

  .filter-tag:hover {
    background: #e2e8f0;
    transform: scale(1.02);
  }

  .filter-tag.active {
    background: #2563eb;
    border-color: #2563eb;
    color: white;
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
      padding: 1rem;
    }

    .search-title {
      font-size: 1.8rem;
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
  }
</style>