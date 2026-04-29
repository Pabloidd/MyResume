<script>
  import { onMount } from 'svelte';
  import { base } from '$app/paths';
  import { browser } from '$app/environment';
  import { beginRemoteLoad, endRemoteLoad } from '$lib/stores/remoteLoading';
  import pdfWorkerSrc from 'pdfjs-dist/build/pdf.worker.min.mjs?url';

  /** @type {string} */
  export let filename;
  /** @type {string} */
  export let title;

  /** @type {HTMLCanvasElement | undefined} */
  let canvas;
  let failed = false;

  /** @param {string} f */
  function pdfUrl(f) {
    return `${base}/${encodeURIComponent(f)}`;
  }

  onMount(() => {
    if (!browser || !canvas) return;

    let cancelled = false;

    (async () => {
      beginRemoteLoad();
      try {
        const pdfjs = await import('pdfjs-dist');
        pdfjs.GlobalWorkerOptions.workerSrc = pdfWorkerSrc;

        const url = pdfUrl(filename);
        const loadingTask = pdfjs.getDocument({
          url,
          disableRange: true,
          withCredentials: false,
        });

        const pdf = await loadingTask.promise;
        if (cancelled) return;

        const page = await pdf.getPage(1);
        if (cancelled) return;

        const maxW = 340;
        const maxH = 280;
        const baseViewport = page.getViewport({ scale: 1 });
        const scale = Math.min(maxW / baseViewport.width, maxH / baseViewport.height, 1.5);
        const viewport = page.getViewport({ scale });

        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        const dpr = Math.min(window.devicePixelRatio || 1, 2);
        canvas.width = Math.floor(viewport.width * dpr);
        canvas.height = Math.floor(viewport.height * dpr);
        canvas.style.width = `${Math.floor(viewport.width)}px`;
        canvas.style.height = `${Math.floor(viewport.height)}px`;

        await page
          .render({
            canvasContext: ctx,
            viewport,
            transform: dpr !== 1 ? [dpr, 0, 0, dpr, 0, 0] : undefined,
          })
          .promise;
      } catch (e) {
        console.error('PDF preview:', e);
        failed = true;
      } finally {
        endRemoteLoad();
      }
    })();

    return () => {
      cancelled = true;
    };
  });
</script>

<div
  class="thumb"
  class:failed
  on:contextmenu|preventDefault
  role="presentation"
>
  {#if failed}
    <span class="fallback">Не удалось показать превью</span>
  {:else}
    <canvas bind:this={canvas} class="cv-canvas" aria-label="Превью первой страницы: {title}"></canvas>
  {/if}
</div>

<style>
  .thumb {
    min-height: 200px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #e8eef3;
  }

  .thumb.failed {
    padding: 1rem;
  }

  .cv-canvas {
    display: block;
    max-width: 100%;
    height: auto;
    user-select: none;
    pointer-events: none;
  }

  .fallback {
    font-size: 0.9rem;
    color: rgba(29, 53, 87, 0.55);
    text-align: center;
  }
</style>
