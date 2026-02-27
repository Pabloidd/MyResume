<div class="search-page">
  <!-- Заголовок -->
  <div class="search-header">
    <h1 class="search-title">НАЙДИТЕ ИДЕАЛЬНОЕ РЕЗЮМЕ</h1>
    <p class="search-subtitle">ПОИСК ПО КЛЮЧЕВЫМ СЛОВАМ, ДОЛЖНОСТИ ИЛИ НАВЫКАМ.</p>
  </div>

  <!-- Поисковая строка -->
  <div class="search-bar">
    <input type="text" class="search-input" placeholder="Введите ключевые слова...">
    <button class="search-button">Найти</button>
  </div>

  <!-- Категории -->
  <div class="categories-section">
    <div class="categories-grid">
      <span class="category-tag">INTERFACE DESIGN</span>
      <span class="category-tag">PROJECT MANAGEMENT</span>
      <span class="category-tag">MARKETING</span>
      <span class="category-tag">COPYWRITING</span>
      <span class="category-tag">NODE.JS</span>
      <span class="category-tag">DEVOPs</span>
      <span class="category-tag">PYTHON</span>
      <span class="category-tag">DATA SCIENCE</span>
      <span class="category-tag">React Native</span>
    </div>
  </div>

  <!-- Топ резюме -->
  <section class="top-resumes-section">
    <h2 class="section-title">Топ Резюме</h2>
    
    <div class="resumes-grid">
      <!-- Карточка 1 -->
      <div class="resume-card">
        <div class="card-content">
          <h3 class="card-title">SENIOR PRODUCT DESIGNER</h3>
          <div class="card-tags">
            <span class="tag">INTERFACE DESIGN</span>
            <span class="tag">FIGMA</span>
            <span class="tag">PROTOTYPING</span>
          </div>
        </div>
        <button class="download-btn">
          <span class="download-icon">⬇️</span>
          <span class="download-text">Скачать</span>
        </button>
      </div>

      <!-- Карточка 2 -->
      <div class="resume-card">
        <div class="card-content">
          <h3 class="card-title">FULL STACK DEVELOPER</h3>
          <div class="card-tags">
            <span class="tag">NODE.JS</span>
            <span class="tag">REACT NATIVE</span>
            <span class="tag">DEVOPs</span>
          </div>
        </div>
        <button class="download-btn">
          <span class="download-icon">⬇️</span>
          <span class="download-text">Скачать</span>
        </button>
      </div>

      <!-- Карточка 3 -->
      <div class="resume-card">
        <div class="card-content">
          <h3 class="card-title">PROJECT MANAGER</h3>
          <div class="card-tags">
            <span class="tag">PROJECT MANAGEMENT</span>
            <span class="tag">AGILE</span>
            <span class="tag">SCRUM</span>
          </div>
        </div>
        <button class="download-btn">
          <span class="download-icon">⬇️</span>
          <span class="download-text">Скачать</span>
        </button>
      </div>
    </div>
  </section>
</div>

<style>
  .search-page {
    max-width: 1200px;
    margin: 0 auto;
    padding: 2rem;
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    color: #333;
  }

  /* Заголовок */
  .search-header {
    margin-bottom: 2.5rem;
  }

  .search-title {
    font-size: 2.5rem;
    font-weight: bold;
    margin: 0 0 0.5rem 0;
    color: #000;
    letter-spacing: 1px;
  }

  .search-subtitle {
    font-size: 1.35rem;
    color: #475569;
    font-weight: 500;
    margin: 0;
  }

  /* Поисковая строка */
  .search-bar {
    display: flex;
    gap: 1rem;
    margin-bottom: 3rem;
    align-items: center;
  }

  .search-input {
    flex: 1;
    padding: 1rem 1.5rem;
    font-size: 1.35rem;
    border: 2px solid #e2e8f0;
    border-radius: 12px;
    outline: none;
    transition: all 0.2s;
    background: white;
  }

  .search-input:focus {
    border-color: #2563eb;
    box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
  }

  .search-button {
    background-color: rgba(193, 18, 31, 0.63);
    border: none;
    border-radius: 8px;
    padding: 1rem 2.5rem;
    color: white;
    font-size: 1.35rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.2s;
    white-space: nowrap;
  }

  .search-button:hover {
    background-color: rgba(255, 0, 17, 0.63);
    transform: translateY(-1px);
  }

  /* Категории */
  .categories-section {
    margin-bottom: 3rem;
    padding: 2rem;
    background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
    border-radius: 20px;
    border: 1px solid rgba(102, 155, 188, 0.2);
  }

  .categories-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    justify-content: center;
  }

  .category-tag {
    background: #e0f2fe;
    color: #0369a1;
    padding: 0.5rem 1.25rem;
    border-radius: 999px;
    font-size: 1.1rem;
    font-weight: 600;
    letter-spacing: 0.5px;
    white-space: nowrap;
    transition: all 0.2s;
    cursor: default;
  }

  .category-tag:hover {
    transform: scale(1.05);
    background: #bae6fd;
  }

  /* Топ резюме */
  .top-resumes-section {
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
    gap: 1rem;
  }

  .card-title {
    font-size: 1.5rem;
    font-weight: bold;
    margin: 0;
    color: #1e293b;
    border-bottom: 2px solid rgba(102, 155, 188, 0.2);
    padding-bottom: 0.75rem;
  }

  .card-tags {
    display: flex;
    flex-wrap: wrap;
    gap: 0.75rem;
  }

  .tag {
    background: #f1f5f9;
    color: #475569;
    padding: 0.35rem 1rem;
    border-radius: 999px;
    font-size: 0.9rem;
    font-weight: 600;
    letter-spacing: 0.3px;
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

  .download-icon {
    font-size: 1.1rem;
  }

  .download-text {
    font-size: 0.95rem;
  }

  /* ===== АДАПТИВНОСТЬ ===== */

  /* 1300px */
  @media (max-width: 1300px) {
    .search-title {
      font-size: 2.2rem;
    }
    
    .search-subtitle {
      font-size: 1.2rem;
    }
    
    .search-input,
    .search-button {
      font-size: 1.2rem;
    }
    
    .card-title {
      font-size: 1.4rem;
    }
  }

  /* 1080px */
  @media (max-width: 1080px) {
    .resumes-grid {
      gap: 1.5rem;
    }
    
    .card-title {
      font-size: 1.3rem;
    }
    
    .category-tag {
      font-size: 1rem;
      padding: 0.4rem 1rem;
    }
  }

  /* 900px */
  @media (max-width: 900px) {
    .search-bar {
      flex-direction: column;
      align-items: stretch;
    }
    
    .search-button {
      width: 100%;
    }
    
    .resumes-grid {
      grid-template-columns: repeat(2, 1fr);
    }
    
    .categories-section {
      padding: 1.5rem;
    }
  }

  /* 650px */
  @media (max-width: 650px) {
    .search-title {
      font-size: 1.8rem;
    }
    
    .search-subtitle {
      font-size: 1rem;
    }
    
    .search-input,
    .search-button {
      font-size: 1rem;
      padding: 0.8rem 1.2rem;
    }
    
    .categories-grid {
      gap: 0.75rem;
    }
    
    .category-tag {
      font-size: 0.9rem;
      padding: 0.4rem 1rem;
    }
    
    .resumes-grid {
      grid-template-columns: 1fr;
    }
    
    .resume-card {
      flex-direction: column;
    }
    
    .card-title {
      font-size: 1.25rem;
    }
    
    .download-btn {
      width: 100%;
    }
  }

  /* 475px */
  @media (max-width: 475px) {
    .search-page {
      padding: 1rem;
    }
    
    .search-title {
      font-size: 1.5rem;
    }
    
    .search-subtitle {
      font-size: 0.9rem;
    }
    
    .search-input,
    .search-button {
      font-size: 0.9rem;
      padding: 0.7rem 1rem;
    }
    
    .categories-section {
      padding: 1rem;
    }
    
    .category-tag {
      font-size: 0.8rem;
      padding: 0.3rem 0.8rem;
    }
    
    .section-title {
      font-size: 1.5rem;
    }
    
    .resume-card {
      padding: 1rem;
    }
    
    .card-title {
      font-size: 1.1rem;
    }
    
    .tag {
      font-size: 0.8rem;
      padding: 0.25rem 0.75rem;
    }
    
    .download-btn {
      padding: 0.6rem 0.8rem;
    }
    
    .download-icon {
      font-size: 1rem;
    }
    
    .download-text {
      font-size: 0.85rem;
    }
  }
</style>