<script>


    import { page } from '$app/state';
    import { base } from '$app/paths';
    import HeaderMainPage from "$lib/components/layout/HeaderMain.svelte";
    import HeaderStandart from "$lib/components/layout/HeaderStandard.svelte";
    import Footer from "$lib/components/layout/Footer.svelte";
    import { browser } from '$app/environment';
  
    if (browser) {
        document.body.classList.add('main-layout');
        document.body.classList.remove('auth-layout');
    }

    $: isMainPage = page.url.pathname === base + '/' || page.url.pathname === base;
</script>
{#if isMainPage}<!--если страница главная-->
    <HeaderMainPage />
{:else}
    <HeaderStandart />
{/if}
<main>
    <slot />
</main>
<Footer />



<style>
    :global(body.main-layout) {
        margin: 0 auto;
        width: 100%;
        max-width: min(1440px, 100%);
        background-color: rgba(241, 250, 238, 1);
        box-shadow: 0 0 25px 0 rgba(29, 53, 87, 0.4);
        box-sizing: border-box;
    }

    main {
        background: linear-gradient(135deg, rgba(255, 255, 255, 1) 0%, rgba(102, 155, 188, 1) 750%);
        padding: 1rem clamp(1rem, 4vw, 5rem);
        width: 100%;
        max-width: 100%;
        overflow-x: hidden;
    }

    @media (max-width: 1500px) {
        :global(body.main-layout) {
            max-width: min(1240px, 100%);
        }
    }

    @media (max-width: 1300px) {
        :global(body.main-layout) {
            max-width: min(1040px, 100%);
        }
    }

    @media (max-width: 1080px) {
        :global(body.main-layout) {
            max-width: min(960px, 100%);
        }
    }

    @media (max-width: 900px) {
        :global(body.main-layout) {
            max-width: 100%;
        }

        main {
            padding: 1rem 1.25rem;
        }
    }

    @media (max-width: 650px) {
        :global(body.main-layout) {
            max-width: 100%;
            box-shadow: none;
        }

        main {
            padding: 0.85rem 1rem;
        }
    }

    @media (max-width: 380px) {
        main {
            padding: 0.65rem 0.75rem;
        }
    }
</style>
