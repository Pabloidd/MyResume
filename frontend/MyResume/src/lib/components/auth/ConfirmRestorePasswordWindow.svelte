<script>
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';
    import { page } from '$app/stores';
    import { authApi } from '$lib/authApi';
    import toast from 'svelte-french-toast';

    let code = $state('');
    let newPassword = $state('');
    let confirmPassword = $state('');
    let isLoading = $state(false);

    const email = $page.url.searchParams.get('email') || '';

    async function handleResetPassword() {
        if (!code || !newPassword || !confirmPassword) {
            toast.error('Заполните все поля');
            return;
        }

        if (newPassword !== confirmPassword) {
            toast.error('Пароли не совпадают');
            return;
        }

        if (newPassword.length < 6) {
            toast.error('Пароль должен быть не менее 6 символов');
            return;
        }

        isLoading = true;
        try {
            await authApi.resetPassword(email, code, newPassword);
            toast.success('Пароль успешно изменен!');
            goto(`${base}/SignIn`);
        } catch (error) {
            toast.error(error.message || 'Ошибка сброса пароля');
        } finally {
            isLoading = false;
        }
    }
</script>

<div class="confirm-container">
    <div class="glass-card">
        <h1 class="title">Новый пароль</h1>
        <p class="subtitle">Введите код подтверждения и новый пароль для вашей учетной записи.</p>
        
        <form on:submit|preventDefault={handleResetPassword}>
            <div class="form-group">
                <label for="code">Код из письма</label>
                <input
                    type="text"
                    id="code"
                    bind:value={code}
                    placeholder="000000"
                    maxlength="6"
                    required
                />
            </div>
            
            <div class="form-group">
                <label for="password">Новый пароль</label>
                <input
                    type="password"
                    id="password"
                    bind:value={newPassword}
                    placeholder="••••••••"
                    required
                />
            </div>
            
            <div class="form-group">
                <label for="confirmPassword">Повторите пароль</label>
                <input
                    type="password"
                    id="confirmPassword"
                    bind:value={confirmPassword}
                    placeholder="••••••••"
                    required
                />
            </div>
            
            <button type="submit" class="submit-btn" disabled={isLoading}>
                {#if isLoading}
                    <span class="spinner"></span>
                {:else}
                    Сбросить пароль
                {/if}
            </button>
        </form>
    </div>
</div>

<style>
    .confirm-container {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 80vh;
        padding: 20px;
    }

    .glass-card {
        background: white;
        border: 1px solid rgba(102, 155, 188, 0.2);
        border-radius: 30px;
        padding: 50px;
        width: 100%;
        max-width: 480px;
        box-shadow: 0 20px 40px rgba(29, 53, 87, 0.1);
        color: #1d3557;
    }

    .title {
        font-size: 2.5rem;
        font-weight: 800;
        margin-bottom: 0.5rem;
        text-align: center;
        color: #1d3557;
    }

    .subtitle {
        text-align: center;
        color: #457b9d;
        margin-bottom: 2.5rem;
        font-size: 1rem;
        line-height: 1.6;
    }

    .form-group {
        margin-bottom: 1.5rem;
    }

    label {
        display: block;
        font-size: 0.9rem;
        font-weight: 600;
        margin-bottom: 0.5rem;
        color: #457b9d;
        text-transform: uppercase;
        letter-spacing: 1px;
    }

    input {
        width: 100%;
        padding: 12px 20px;
        background: #f8fafc;
        border: 2px solid #e2e8f0;
        border-radius: 12px;
        color: #1d3557;
        font-size: 1rem;
        transition: all 0.3s ease;
        box-sizing: border-box;
    }

    input::placeholder {
        color: #94a3b8;
    }

    input:focus {
        outline: none;
        border-color: #457b9d;
        background: white;
        box-shadow: 0 0 0 4px rgba(69, 123, 157, 0.1);
    }

    #code {
        text-align: center;
        letter-spacing: 0.5rem;
        font-size: 1.5rem;
        font-weight: 700;
        background: #f1f5f9;
    }

    .submit-btn {
        width: 100%;
        padding: 14px;
        background: #457b9d;
        color: white;
        border: none;
        border-radius: 12px;
        font-size: 1.1rem;
        font-weight: 700;
        cursor: pointer;
        transition: all 0.3s ease;
        margin-top: 1rem;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .submit-btn:hover:not(:disabled) {
        transform: translateY(-2px);
        box-shadow: 0 10px 20px rgba(69, 123, 157, 0.2);
        background: #1d3557;
    }

    .submit-btn:disabled {
        opacity: 0.7;
        cursor: not-allowed;
    }

    .spinner {
        width: 20px;
        height: 20px;
        border: 2px solid rgba(118, 75, 162, 0.3);
        border-top-color: #764ba2;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
        to { transform: rotate(360deg); }
    }
</style>
