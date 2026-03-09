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
    :global(body.main-layout){
        margin: 0 auto;
        background-color: rgba(241, 250, 238, 1);
        max-width: 1440px;
        box-shadow: 0 0  25px 0px rgba(29, 53, 87, 0.4);
    }

    main{
        background: linear-gradient(135deg, rgba(255, 255, 255, 1) 0%, rgba(102, 155, 188, 1) 750%);
        padding: 1rem 5rem;
    }

    @media (max-width: 1500px){
        :global(body.main-layout){
        max-width: 1240px;
        }
    }
    @media (max-width: 1300px){
        :global(body.main-layout){
        max-width: 1040px;
        }
    }

    @media (max-width: 1080px){
        :global(body.main-layout){
        max-width: 840px;
        }
    }

    @media (max-width: 900px){
        :global(body.main-layout){
        max-width: 640px;
        }
    }

    @media (max-width: 650px){
        :global(body.main-layout){
        max-width: 440px;
        }
        main{
            padding: 1rem 1.25rem;
        }
    }
    @media (max-width: 475px){
        :global(body.main-layout){
        max-width: 300px;
        }
    }
</style>
