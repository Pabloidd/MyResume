<script>
  import { browser } from '$app/environment';
  if (browser) {
    document.body.classList.add('auth-layout');
    document.body.classList.remove('main-layout');
  }
</script>

<main>
    <slot />
</main>

<style>
    /* Стили для body ТОЛЬКО на страницах регистрации */
  :global(body.auth-layout) {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    height: 98vh;
  }

</style>