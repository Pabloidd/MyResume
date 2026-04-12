<script>
    import { onMount } from 'svelte';
    import { page } from '$app/stores';
    import { goto, beforeNavigate } from '$app/navigation';
    import { Toaster } from 'svelte-french-toast';
    import { auth } from '$lib/authStore';
    import { authApi } from '$lib/authApi';
    import '$lib/../app.css'; // Assuming there might be an app.css or I'll create one

    // Routes that don't require authentication
    const publicRoutes = ['/', '/registration', '/articles', '/SignIn'];
    import toast from 'svelte-french-toast';

    async function checkUserAuth() {
        try {
            const data = await authApi.check();
            if (data && data.user) {
                auth.setUser(data.user);
            } else {
                auth.logout();
            }
        } catch (e) {
            auth.logout();
        } finally {
            auth.setLoading(false);
        }
    }

    onMount(() => {
        checkUserAuth();
    });

    // Global guard logic
    beforeNavigate(({ to, cancel }) => {
        if (!to) return;
        
        const targetPath = to.url.pathname;
        const isPublic = publicRoutes.some(route => 
            targetPath === route || targetPath.startsWith(`${route}/`)
        );

        // Accessing the store value within a function in Svelte 5
        let isAuthenticated = false;
        auth.subscribe(v => isAuthenticated = v.isAuthenticated)();

        if (!isPublic && !isAuthenticated) {
            // Uncomment if you want strict redirection immediately
            // cancel();
            // goto('/SignIn');
        }
    });

    // Reactive redirection if user data is loaded and they are on a protected page
    $: if (!$auth.isLoading && !$auth.isAuthenticated) {
        const path = $page.url.pathname;
        const isPublic = publicRoutes.some(route => 
            path === route || path.startsWith(`${route}/`)
        );
        
        if (!isPublic) {
            goto('/SignIn');
        }
    }

    // Role-based protection for /search
    $: if (!$auth.isLoading && $auth.isAuthenticated && $auth.user) {
        const path = $page.url.pathname;
        if (path.startsWith('/search') && $auth.user.role === 'user') {
            toast.error('Раздел «Поиск резюме» доступен только пользователям с Премиум-аккаунтом.');
            goto('/profile');
        }
    }
</script>

<Toaster />

{#if $auth.isLoading}
    <div class="loading-screen">
        <div class="spinner"></div>
    </div>
{:else}
    <slot />
{/if}

<style>
    :global(body) {
        margin: 0;
        padding: 0;
        min-height: 100vh;
    }

    .loading-screen {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh;
        background: white;
    }

    .spinner {
        width: 50px;
        height: 50px;
        border: 5px solid #f3f3f3;
        border-top: 5px solid #667eea;
        border-radius: 50%;
        animation: spin 1s linear infinite;
    }

    @keyframes spin {
        0% { transform: rotate(0deg); }
        100% { transform: rotate(360deg); }
    }
</style>
