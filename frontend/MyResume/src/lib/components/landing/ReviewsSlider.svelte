<script>
  // Simple Mock Reviews Data
  const reviews = [
    {
      id: 1,
      name: "Анна С.",
      role: "UX/UI Дизайнер",
      text: "Отличный сервис! Создала резюме за 15 минут, и уже на следующий день получила приглашение на собеседование. Очень удобный интерфейс.",
      rating: 5
    },
    {
      id: 2,
      name: "Михаил И.",
      role: "Разработчик",
      text: "Понравились шаблоны и простота использования. Моё старое резюме выглядело скучно, а теперь выглядит очень профессионально и современно.",
      rating: 5
    },
    {
      id: 3,
      name: "Елена В.",
      role: "Менеджер по продажам",
      text: "Спасибо за экспертные советы по заполнению! Благодаря платформе смогла подчеркнуть свои сильные стороны. Рекомендую всем!",
      rating: 4
    }
  ];

  let currentIndex = 0;

  function nextReview() {
    currentIndex = (currentIndex + 1) % reviews.length;
  }

  function prevReview() {
    currentIndex = (currentIndex - 1 + reviews.length) % reviews.length;
  }
</script>

<section class="reviews-section">
  <div class="reviews-container">
    <h2 class="section-title">Отзывы наших пользователей</h2>
    
    <div class="slider-wrapper">
      <button class="nav-btn prev" on:click={prevReview} aria-label="Предыдущий отзыв">
        &#10094;
      </button>

      <div class="review-card">
        <div class="stars">
          {#each Array(reviews[currentIndex].rating) as _}
            <span class="star">★</span>
          {/each}
          {#each Array(5 - reviews[currentIndex].rating) as _}
            <span class="star empty">☆</span>
          {/each}
        </div>
        <p class="review-text">"{reviews[currentIndex].text}"</p>
        <div class="review-author">
          <span class="author-name">{reviews[currentIndex].name}</span>
          <span class="author-role">{reviews[currentIndex].role}</span>
        </div>
      </div>

      <button class="nav-btn next" on:click={nextReview} aria-label="Следующий отзыв">
        &#10095;
      </button>
    </div>

    <div class="slider-dots">
      {#each reviews as _, i}
        <button 
          class="dot {i === currentIndex ? 'active' : ''}" 
          on:click={() => currentIndex = i}
          aria-label="Перейти к отзыву {i + 1}"
        ></button>
      {/each}
    </div>
  </div>
</section>

<style>
  .reviews-section {
    padding: 4rem 1rem;
    background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  }

  .reviews-container {
    max-width: 800px;
    margin: 0 auto;
    text-align: center;
  }

  .section-title {
    font-size: 2.5rem;
    font-weight: bold;
    color: #1e293b;
    margin-bottom: 3rem;
  }

  .slider-wrapper {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 2rem;
    position: relative;
  }

  .review-card {
    background: white;
    padding: 3rem;
    border-radius: 20px;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.05);
    flex: 1;
    min-height: 200px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    transition: all 0.3s ease;
  }

  .stars {
    color: #fbbf24;
    font-size: 1.5rem;
    margin-bottom: 1.5rem;
  }

  .star.empty {
    color: #cbd5e1;
  }

  .review-text {
    font-size: 1.25rem;
    color: #475569;
    line-height: 1.6;
    margin-bottom: 2rem;
    font-style: italic;
  }

  .author-name {
    display: block;
    font-weight: bold;
    font-size: 1.1rem;
    color: #1e293b;
  }

  .author-role {
    display: block;
    font-size: 0.9rem;
    color: #64748b;
    margin-top: 0.25rem;
  }

  .nav-btn {
    background: white;
    border: none;
    width: 50px;
    height: 50px;
    border-radius: 50%;
    font-size: 1.5rem;
    color: #2563eb;
    cursor: pointer;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
    transition: all 0.2s;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .nav-btn:hover {
    background: #2563eb;
    color: white;
    transform: scale(1.1);
  }

  .slider-dots {
    display: flex;
    justify-content: center;
    gap: 0.75rem;
    margin-top: 2rem;
  }

  .dot {
    width: 12px;
    height: 12px;
    border-radius: 50%;
    background: #cbd5e1;
    border: none;
    cursor: pointer;
    transition: all 0.2s;
    padding: 0;
  }

  .dot.active {
    background: #2563eb;
    transform: scale(1.2);
  }

  @media (max-width: 650px) {
    .slider-wrapper {
      gap: 1rem;
    }

    .review-card {
      padding: 2rem 1.5rem;
    }

    .section-title {
      font-size: 2rem;
      margin-bottom: 2rem;
    }

    .review-text {
      font-size: 1.1rem;
    }

    .nav-btn {
      width: 40px;
      height: 40px;
      font-size: 1.25rem;
    }
  }
</style>
