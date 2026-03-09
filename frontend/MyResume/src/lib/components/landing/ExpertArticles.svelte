<script>
  // Mock Expert Articles Data
  const articles = [
    {
      id: 1,
      title: "Как составить идеальное резюме в 2024 году",
      description: "Основные тренды, ошибки, которых стоит избегать, и секреты, которые помогут вам выделиться среди конкурентов.",
      author: "Антон Экспертов",
      category: "Советы",
      link: "/articles/resume"
    },
    {
      id: 2,
      title: "Секреты успешного прохождения собеседования",
      description: "К чему готовиться, как отвечать на каверзные вопросы HR и как преодолеть волнение перед важной встречей.",
      author: "Мария Кадровая",
      category: "Собеседование",
      link: "/articles/interview"
    },
    {
      id: 3,
      title: "Как правильно описать свои навыки и достижения",
      description: "Фокус на результатах, а не на процессе. Учимся использовать глаголы действия и измеримые показатели.",
      author: "Игорь Резюмейкер",
      category: "Оформление",
      link: "/articles/skills"
    }
  ];
</script>

<section class="articles-section">
  <div class="articles-container">
    <div class="section-header">
      <h2 class="section-title">Статьи наших экспертов</h2>
      <p class="section-subtitle">Полезные советы и рекомендации для успешной карьеры</p>
    </div>
    
    <div class="articles-grid">
      {#each articles as article}
        <a href={article.link} class="article-card">
          <div class="article-category">{article.category}</div>
          <h3 class="article-title">{article.title}</h3>
          <p class="article-description">{article.description}</p>
          <div class="article-footer">
            <span class="article-author">Автор: {article.author}</span>
            <span class="read-more">Читать &rarr;</span>
          </div>
        </a>
      {/each}
    </div>
  </div>
</section>

<style>
  .articles-section {
    padding: 5rem 1rem;
    background-color: #ffffff;
  }

  .articles-container {
    max-width: 1200px;
    margin: 0 auto;
  }

  .section-header {
    text-align: center;
    margin-bottom: 4rem;
  }

  .section-title {
    font-size: 2.5rem;
    font-weight: bold;
    color: #1e293b;
    margin: 0 0 1rem 0;
  }

  .section-subtitle {
    font-size: 1.25rem;
    color: #64748b;
    margin: 0;
  }

  .articles-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 2rem;
  }

  .article-card {
    background: #f8fafc;
    border-radius: 16px;
    padding: 2rem;
    text-decoration: none;
    color: inherit;
    transition: all 0.3s ease;
    border: 1px solid #e2e8f0;
    display: flex;
    flex-direction: column;
    height: 100%;
    box-sizing: border-box;
  }

  .article-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 12px 24px rgba(0, 0, 0, 0.08);
    border-color: #2563eb;
    background: white;
  }

  .article-category {
    font-size: 0.85rem;
    font-weight: 600;
    color: #2563eb;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    margin-bottom: 1rem;
  }

  .article-title {
    font-size: 1.5rem;
    font-weight: bold;
    color: #0f172a;
    line-height: 1.4;
    margin: 0 0 1rem 0;
  }

  .article-description {
    font-size: 1rem;
    color: #475569;
    line-height: 1.6;
    margin: 0 0 2rem 0;
    flex-grow: 1;
  }

  .article-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-top: 1.5rem;
    border-top: 1px solid #e2e8f0;
    margin-top: auto;
  }

  .article-author {
    font-size: 0.9rem;
    color: #64748b;
  }

  .read-more {
    font-weight: 600;
    color: #2563eb;
    font-size: 0.95rem;
  }

  @media (max-width: 768px) {
    .section-title {
      font-size: 2rem;
    }
    
    .section-subtitle {
      font-size: 1.1rem;
    }
    
    .article-card {
      padding: 1.5rem;
    }
    
    .article-title {
      font-size: 1.25rem;
    }
  }
</style>
