<script>
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';
    import { page } from '$app/stores';
    import { authApi } from '$lib/authApi';
    import { auth } from '$lib/authStore';
    import toast from 'svelte-french-toast';

    let code = $state('');
    let isLoading = $state(false);
    let isResending = $state(false);
    
    // Get email from query parameter
    const email = $page.url.searchParams.get('email') || '';

    async function handleVerify() {
        if (!code || code.length < 6) {
            toast.error('Введите 6-значный код');
            return;
        }

        if (!email) {
            toast.error('Email не найден. Попробуйте зарегистрироваться заново.');
            return;
        }

        isLoading = true;
        try {
            const data = await authApi.verify(email, code);
            toast.success('Email успешно подтвержден!');
            auth.setUser(data.user);
            goto(`${base}/`); // Redirect to home or dashboard
        } catch (error) {
            toast.error(error.message || 'Ошибка подтверждения');
        } finally {
            isLoading = false;
        }
    }

    async function handleResend() {
        if (!email) return;
        
        isResending = true;
        try {
            await authApi.resendCode(email);
            toast.success('Новый код отправлен!');
        } catch (error) {
            toast.error(error.message || 'Не удалось отправить код');
        } finally {
            isResending = false;
        }
    }
</script>

<div class="verify-container">
    <div class="glass-card">
        <h1 class="title">Подтверждение</h1>
        <p class="message">
            Мы отправили код на <strong>{email || 'ваш e-mail'}</strong>. 
            Введите его ниже для завершения регистрации.
        </p>
        
        <form on:submit|preventDefault={handleVerify}>
            <div class="form-group">
                <label for="code">Код подтверждения</label>
                <input
                    type="text"
                    id="code"
                    bind:value={code}
                    placeholder="000000"
                    maxlength="6"
                    required
                />
            </div>
            
            <button type="submit" class="submit-btn" disabled={isLoading}>
                {#if isLoading}
                    <span class="spinner"></span>
                {:else}
                    Подтвердить
                {/if}
            </button>

            <div class="resend-section">
                Не получили код? 
                <button 
                    type="button" 
                    class="resend-btn" 
                    on:click={handleResend} 
                    disabled={isResending}
                >
                    {isResending ? 'Отправка...' : 'Отправить повторно'}
                </button>
            </div>
        </form>
    </div>
</div>

<style>
    .verify-container {
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

    .message {
        text-align: center;
        color: #457b9d;
        margin-bottom: 2.5rem;
        font-size: 1rem;
        line-height: 1.6;
    }

    .message strong {
        color: #1d3557;
    }

    .form-group {
        margin-bottom: 1.5rem;
    }

    label {
        display: block;
        font-size: 0.9rem;
        font-weight: 600;
        margin-bottom: 1rem;
        color: #457b9d;
        text-transform: uppercase;
        letter-spacing: 1px;
        text-align: center;
    }

    input {
        width: 100%;
        padding: 15px;
        background: #f8fafc;
        border: 2px solid #e2e8f0;
        border-radius: 12px;
        color: #1d3557;
        font-size: 2rem;
        font-weight: 800;
        text-align: center;
        letter-spacing: 0.8rem;
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

    .resend-section {
        text-align: center;
        margin-top: 2rem;
        font-size: 0.9rem;
        color: #457b9d;
    }

    .resend-btn {
        background: none;
        border: none;
        color: #1d3557;
        font-weight: 700;
        cursor: pointer;
        text-decoration: underline;
        margin-left: 5px;
        padding: 0;
    }

    .resend-btn:hover {
        color: #457b9d;
    }

    .resend-btn:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        text-decoration: none;
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
