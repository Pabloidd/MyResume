<script>
    import { onMount } from 'svelte';
    import { page } from '$app/stores';
    import { goto } from '$app/navigation';

    let article = null;
    let loading = true;
    let error = null;

    const API_BASE_URL = 'http://localhost:5052';

    // Получаем ID статьи из URL
    $: articleId = $page.params.id;

    onMount(async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/api/articles/${articleId}`);

            if (!response.ok) {
                if (response.status === 404) {
                    throw new Error('Статья не найдена');
                }
                throw new Error('Ошибка загрузки');
            }

            article = await response.json();

        } catch (err) {
            console.error('Ошибка:', err);
            error = err.message;
        } finally {
            loading = false;
        }
    });

    function goBack() {
        goto('/');
    }
</script>

<svelte:head>
    <title>{article ? article.title : 'Загрузка...'} | Конструктор резюме</title>
</svelte:head>

{#if loading}
    <div class="loading-container">
        <div class="spinner"></div>
        <p>Загрузка статьи...</p>
    </div>
{:else if error}
    <div class="error-container">
        <h1>Ошибка</h1>
        <p>{error}</p>
        <button class="back-btn" on:click={goBack}>Вернуться на главную</button>
    </div>
{:else}
    <article class="article-page">
        <div class="article-container">
            <button class="back-btn" on:click={goBack}>
                ← Назад к статьям
            </button>

            <div class="article-header">
                <div class="article-meta">
                    <span class="article-author">{article.author}</span>
                    <span class="article-category">Статья</span>
                </div>
                <h1 class="article-title">{article.title}</h1>
            </div>

            <div class="article-content">
                {@html article.content}
            </div>

            <div class="article-footer">
                <button class="back-btn-bottom" on:click={goBack}>
                    ← Вернуться к списку статей
                </button>
            </div>
        </div>
    </article>
{/if}

<style>
    .loading-container, .error-container {
        max-width: 800px;
        margin: 4rem auto;
        text-align: center;
        padding: 2rem;
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

    .error-container h1 {
        font-size: 2rem;
        color: #ef4444;
        margin-bottom: 1rem;
    }

    .error-container p {
        color: #475569;
        margin-bottom: 2rem;
    }

    .article-page {
        background: #ffffff;
        min-height: 100vh;
    }

    .article-container {
        max-width: 800px;
        width: 100%;
        margin: 0 auto;
        padding: 2rem 1.5rem;
        box-sizing: border-box;
    }

    .back-btn {
        background: none;
        border: none;
        color: #2563eb;
        font-size: 1rem;
        cursor: pointer;
        padding: 0.5rem 0;
        margin-bottom: 2rem;
        font-weight: 500;
    }

    .back-btn:hover {
        text-decoration: underline;
    }

    .back-btn-bottom {
        background: none;
        border: none;
        color: #2563eb;
        font-size: 1rem;
        cursor: pointer;
        padding: 0.75rem 1.5rem;
        margin-top: 2rem;
        font-weight: 500;
        border: 1px solid #2563eb;
        border-radius: 8px;
        transition: all 0.2s;
    }

    .back-btn-bottom:hover {
        background: #2563eb;
        color: white;
    }

    .article-header {
        margin-bottom: 2rem;
    }

    .article-meta {
        display: flex;
        gap: 1rem;
        align-items: center;
        margin-bottom: 1rem;
    }

    .article-author {
        font-size: 0.9rem;
        color: #64748b;
    }

    .article-category {
        font-size: 0.75rem;
        font-weight: 600;
        color: #2563eb;
        text-transform: uppercase;
        letter-spacing: 0.5px;
    }

    .article-title {
        font-size: 2.5rem;
        font-weight: bold;
        color: #1e293b;
        line-height: 1.3;
        margin: 0;
    }

    .article-content {
        font-size: 1.1rem;
        line-height: 1.7;
        color: #334155;
    }

    /* :global — разметка из {@html} не получает scoped-атрибуты Svelte */
    .article-content :global(p) {
        margin-bottom: 1.5rem;
    }

    .article-content :global(h2),
    .article-content :global(h3) {
        margin: 1.75rem 0 0.75rem;
        color: #1e293b;
        line-height: 1.35;
    }

    .article-content :global(h2) {
        font-size: 1.65rem;
    }

    .article-content :global(h3) {
        font-size: 1.35rem;
    }

    .article-content :global(ul),
    .article-content :global(ol) {
        margin: 0 0 1.25rem 1.25rem;
        padding-left: 1.25rem;
    }

    .article-content :global(li) {
        margin-bottom: 0.35rem;
    }

    .article-content :global(a) {
        color: #2563eb;
        text-decoration: underline;
    }

    .article-footer {
        text-align: center;
        margin-top: 3rem;
        padding-top: 2rem;
        border-top: 1px solid #e2e8f0;
    }

    @media (max-width: 768px) {
        .article-container {
            padding: 1.5rem 1rem;
        }

        .article-title {
            font-size: clamp(1.35rem, 5vw, 1.8rem);
        }

        .article-content {
            font-size: 1rem;
        }
    }

    @media (max-width: 480px) {
        .article-container {
            padding: 1rem 0.75rem;
        }

        .article-content :global(ul),
        .article-content :global(ol) {
            margin-left: 0.5rem;
            padding-left: 1rem;
        }

        .article-content :global(h2) {
            font-size: 1.35rem;
        }

        .article-content :global(h3) {
            font-size: 1.15rem;
        }
    }
</style>