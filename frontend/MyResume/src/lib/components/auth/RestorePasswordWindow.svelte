<script>
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';
    import { authApi } from '$lib/authApi';
    import toast from 'svelte-french-toast';

    let email = $state('');
    let isLoading = $state(false);

    async function handleRequestCode() {
        if (!email) {
            toast.error('Введите ваш e-mail');
            return;
        }

        isLoading = true;
        try {
            await authApi.forgotPassword(email);
            toast.success('Код для сброса пароля отправлен!');
            goto(`${base}/SignIn/restorePassword/confirm?email=${encodeURIComponent(email)}`);
        } catch (error) {
            toast.error(error.message || 'Ошибка запроса');
        } finally {
            isLoading = false;
        }
    }
</script>

<div class="restore-container">
    <div class="glass-card">
        <h1 class="title">Сброс пароля</h1>
        <p class="message">
            Введите вашу почту, и мы отправим вам инструкции по восстановлению доступа.
        </p>
        
        <form on:submit|preventDefault={handleRequestCode}>
            <div class="form-group">
                <label for="email">E-mail адрес</label>
                <input
                    type="email"
                    id="email"
                    bind:value={email}
                    placeholder="example@mail.com"
                    required
                />
            </div>
            
            <button type="submit" class="submit-btn" disabled={isLoading}>
                {#if isLoading}
                    <span class="spinner"></span>
                {:else}
                    Получить код
                {/if}
            </button>

            <div class="footer-links">
                Вспомнили пароль? <a href="{base}/SignIn">Вернуться</a>
            </div>
        </form>
    </div>
</div>

<style>
    .restore-container {
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

    .form-group {
        margin-bottom: 2rem;
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

    .footer-links {
        text-align: center;
        margin-top: 1.5rem;
        font-size: 0.9rem;
        color: #457b9d;
    }

    .footer-links a {
        color: #1d3557;
        text-decoration: none;
        font-weight: 600;
        margin-left: 5px;
    }

    .footer-links a:hover {
        text-decoration: underline;
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
