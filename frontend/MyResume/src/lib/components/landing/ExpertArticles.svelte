<script>
  import { onMount } from 'svelte';
  import { beginRemoteLoad, endRemoteLoad } from '$lib/stores/remoteLoading.js';

  let articles = [];
  let loading = true;
  let error = null;
  let currentIndex = 0;
  let slidesToShow = 3;

  const API_BASE_URL = 'http://localhost:5052';

  function updateSlidesToShow() {
    if (window.innerWidth < 640) slidesToShow = 1;
    else if (window.innerWidth < 1024) slidesToShow = 2;
    else slidesToShow = 3;
  }

  function nextSlide() {
    if (currentIndex + slidesToShow < articles.length) {
      currentIndex++;
    }
  }

  function prevSlide() {
    if (currentIndex > 0) {
      currentIndex--;
    }
  }

  function goToSlide(index) {
    currentIndex = index;
  }

  // Отображаемые статьи
  $: visibleArticles = articles.slice(currentIndex, currentIndex + slidesToShow);

  // Количество точек для пагинации
  $: totalDots = Math.max(1, articles.length - slidesToShow + 1);

  onMount(async () => {
    updateSlidesToShow();
    window.addEventListener('resize', updateSlidesToShow);

    beginRemoteLoad();
    try {
      const response = await fetch(`${API_BASE_URL}/api/articles/preview`);

      if (!response.ok) throw new Error('Ошибка загрузки');

      const data = await response.json();

      articles = data.map(article => ({
        id: article.id,
        title: article.title,
        description: article.preview,
        author: article.author,
        link: `/articles/${article.id}`
      }));

    } catch (err) {
      console.error('Ошибка:', err);
      error = 'Не удалось загрузить статьи';
    } finally {
      loading = false;
      endRemoteLoad();
    }
  });
</script>

<section class="articles-section">
  <div class="articles-container">
    <div class="section-header">
      <h2 class="section-title">Статьи наших экспертов</h2>
      <p class="section-subtitle">Полезные советы и рекомендации для успешной карьеры</p>
    </div>

    {#if loading}
      <div class="loading-state">
        <div class="spinner"></div>
        <p>Загрузка статей...</p>
      </div>
    {:else if error}
      <div class="error-state">
        <p>{error}</p>
      </div>
    {:else if articles.length === 0}
      <div class="empty-state">
        <p>Статьи пока не добавлены</p>
      </div>
    {:else}
      <div class="slider-wrapper">
        {#if currentIndex > 0}
          <button class="slider-btn prev" on:click={prevSlide}>
            ‹
          </button>
        {/if}

        <div class="slider-container">
          <div class="articles-grid" style="grid-template-columns: repeat({slidesToShow}, 1fr);">
            {#each visibleArticles as article}
              <a href={article.link} class="article-card">
                <div class="article-category">Статья</div>
                <h3 class="article-title">{article.title}</h3>
                <p class="article-description">{article.description}</p>
                <div class="article-footer">
                  <span class="article-author">{article.author}</span>
                  <span class="read-more">Читать →</span>
                </div>
              </a>
            {/each}
          </div>
        </div>

        {#if currentIndex + slidesToShow < articles.length}
          <button class="slider-btn next" on:click={nextSlide}>
            ›
          </button>
        {/if}
      </div>

      <!-- Индикаторы (точки) -->
      <div class="dots">
        {#each Array(totalDots) as _, i}
          <button
                  class="dot {currentIndex === i ? 'active' : ''}"
                  on:click={() => goToSlide(i)}
          />
        {/each}
      </div>
    {/if}
  </div>
</section>

<style>
  .articles-section {
    padding: 4rem 1rem;
    background: #ffffff;
  }

  .articles-container {
    max-width: 1200px;
    margin: 0 auto;
  }

  .section-header {
    text-align: center;
    margin-bottom: 2.5rem;
  }

  .section-title {
    font-size: 2.2rem;
    font-weight: bold;
    color: #1e293b;
    margin: 0 0 0.75rem 0;
  }

  .section-subtitle {
    font-size: 1.1rem;
    color: #64748b;
    margin: 0;
  }

  /* Слайдер */
  .slider-wrapper {
    position: relative;
    display: flex;
    align-items: center;
    gap: 1rem;
  }

  .slider-container {
    flex: 1;
    overflow: visible;
  }

  .articles-grid {
    display: grid;
    gap: 1.5rem;
    transition: all 0.3s ease;
  }

  .article-card {
    background: #f8fafc;
    border-radius: 16px;
    padding: 1.5rem;
    text-decoration: none;
    color: inherit;
    transition: all 0.3s ease;
    border: 1px solid #e2e8f0;
    display: flex;
    flex-direction: column;
    height: 100%;
    min-height: 260px;
    box-sizing: border-box;
  }

  .article-card:hover {
    transform: translateY(-4px);
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.08);
    border-color: #2563eb;
    background: white;
  }

  .article-category {
    font-size: 0.75rem;
    font-weight: 600;
    color: #2563eb;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    margin-bottom: 0.75rem;
  }

  .article-title {
    font-size: 1.25rem;
    font-weight: bold;
    color: #0f172a;
    line-height: 1.4;
    margin: 0 0 0.75rem 0;
  }

  .article-description {
    font-size: 0.95rem;
    color: #475569;
    line-height: 1.5;
    margin: 0 0 1.25rem 0;
    flex: 1;
  }

  .article-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-top: 1rem;
    border-top: 1px solid #e2e8f0;
    margin-top: auto;
  }

  .article-author {
    font-size: 0.85rem;
    color: #64748b;
  }

  .read-more {
    font-weight: 600;
    color: #2563eb;
    font-size: 0.9rem;
  }

  /* Кнопки слайдера */
  .slider-btn {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: white;
    border: 1px solid #e2e8f0;
    font-size: 1.5rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s;
    color: #1e293b;
    flex-shrink: 0;
  }

  .slider-btn:hover {
    background: #2563eb;
    border-color: #2563eb;
    color: white;
  }

  /* Индикаторы */
  .dots {
    display: flex;
    justify-content: center;
    gap: 0.75rem;
    margin-top: 2rem;
  }

  .dot {
    width: 10px;
    height: 10px;
    border-radius: 50%;
    background: #cbd5e1;
    border: none;
    cursor: pointer;
    transition: all 0.2s;
    padding: 0;
  }

  .dot.active {
    width: 28px;
    border-radius: 5px;
    background: #2563eb;
  }

  /* Состояния */
  .loading-state, .error-state, .empty-state {
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

  .error-state { color: #ef4444; }
  .empty-state { color: #64748b; }

  /* Адаптив */
  @media (max-width: 768px) {
    .articles-section { padding: 2rem 1rem; }
    .section-title { font-size: 1.6rem; }
    .section-subtitle { font-size: 0.9rem; }
    .slider-btn { width: 32px; height: 32px; font-size: 1.2rem; }
    .article-card { padding: 1rem; min-height: 220px; }
    .article-title { font-size: 1rem; }
    .article-description { font-size: 0.85rem; }
    .article-author { font-size: 0.75rem; }
    .read-more { font-size: 0.8rem; }
    .dots { gap: 0.5rem; margin-top: 1rem; }
    .dot { width: 8px; height: 8px; }
    .dot.active { width: 20px; }
  }
</style>