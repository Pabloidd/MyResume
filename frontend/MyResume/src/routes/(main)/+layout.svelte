<script>
    import { page } from '$app/state';
    import HeaderMainPage from "$lib/components/Header_main_page.svelte";
    import HeaderStandart from "$lib/components/Header_standart.svelte";
    import Footer from "$lib/components/Footer.svelte";
    import { browser } from '$app/environment';
  
    if (browser) {
        document.body.classList.add('main-layout');
        document.body.classList.remove('auth-layout');
    }
</script>
{#if page.url.pathname === '/'}<!--если страница главная-->
    <HeaderMainPage />
{:else}
    <HeaderStandart />
{/if}
<main>
    <slot />
</main>
<Footer />



<style>
    :global(body.main-layout){
        margin: 0 auto;
        background-color: rgba(241, 250, 238, 1);
        max-width: 1440px;
        max-height: 1848px;
        box-shadow: 0 0  25px 0px rgba(29, 53, 87, 0.4);
    }
</style>