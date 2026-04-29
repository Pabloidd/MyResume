<script>
  import PdfResumeThumb from './PdfResumeThumb.svelte';

  /** Имена файлов из frontend/MyResume/static */
  const samples = [
    {
      file: 'Head of Digital Marketing_20260426.pdf',
      title: 'Head of Digital Marketing',
      blurb: 'Руководитель digital-маркетинга: стратегия, метрики, команда',
    },
    {
      file: 'Senior Full-Stack Developer_20260426.pdf',
      title: 'Senior Full-Stack Developer',
      blurb: 'Full-stack: продуктовая разработка и сильный технический блок',
    },
    {
      file: 'Senior UX_UI Designer_20260426-1.pdf',
      title: 'Senior UX/UI Designer',
      blurb: 'Дизайн продукта с упором на исследования и системный UI',
    },
  ];
</script>

<section class="showcase" aria-labelledby="showcase-heading">
  <div class="showcase-inner">
    <h2 id="showcase-heading" class="heading">Примеры готовых резюме</h2>
    <p class="sub">
      Первые страницы PDF в том виде, в каком их получает работодатель
    </p>

    <div class="grid">
      {#each samples as item}
        <article class="card">
          <div class="preview-wrap">
            <PdfResumeThumb filename={item.file} title={item.title} />
          </div>
          <div class="card-body">
            <h3 class="card-title">{item.title}</h3>
            <p class="card-blurb">{item.blurb}</p>
          </div>
        </article>
      {/each}
    </div>
  </div>
</section>

<style>
  .showcase {
    padding: 3rem 1rem 3.5rem;
    background: #f8fafc;
    border-top: 1px solid rgba(29, 53, 87, 0.08);
  }

  .showcase-inner {
    max-width: 1200px;
    margin: 0 auto;
  }

  .heading {
    margin: 0 0 0.6rem;
    text-align: center;
    font-size: 2rem;
    font-weight: bold;
    color: rgba(29, 53, 87, 1);
  }

  .sub {
    margin: 0 auto 2.25rem;
    max-width: 640px;
    text-align: center;
    font-size: 1.05rem;
    line-height: 1.5;
    color: rgba(29, 53, 87, 0.72);
  }

  .grid {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }

  .card {
    display: flex;
    flex-direction: column;
    background: #fff;
    border-radius: 16px;
    overflow: hidden;
    border: 1px solid rgba(29, 53, 87, 0.12);
    box-shadow: 0 8px 24px rgba(29, 53, 87, 0.06);
    transition:
      transform 0.2s ease,
      box-shadow 0.2s ease;
  }

  .card:hover {
    transform: translateY(-4px);
    box-shadow: 0 16px 36px rgba(29, 53, 87, 0.1);
  }

  .preview-wrap {
    position: relative;
    height: 300px;
    overflow: hidden;
    display: flex;
    align-items: flex-start;
    justify-content: center;
  }

  .preview-wrap :global(.thumb) {
    width: 100%;
    height: 100%;
    align-items: flex-start;
    padding-top: 0.5rem;
    box-sizing: border-box;
  }

  .card-body {
    padding: 1.1rem 1.25rem 1.25rem;
    display: flex;
    flex-direction: column;
    flex: 1;
    gap: 0.35rem;
  }

  .card-title {
    margin: 0;
    font-size: 1.1rem;
    font-weight: 700;
    color: rgba(29, 53, 87, 1);
  }

  .card-blurb {
    margin: 0;
    font-size: 0.9rem;
    line-height: 1.45;
    color: rgba(29, 53, 87, 0.65);
    flex: 1;
  }

  @media (max-width: 1024px) {
    .grid {
      grid-template-columns: 1fr;
      max-width: 420px;
      margin: 0 auto;
    }

    .preview-wrap {
      height: 280px;
    }
  }

  @media (max-width: 650px) {
    .showcase {
      padding: 2rem 1rem 2.5rem;
    }

    .heading {
      font-size: 1.5rem;
    }

    .sub {
      font-size: 0.95rem;
      margin-bottom: 1.75rem;
    }
  }
</style>
