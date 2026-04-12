const BASE_URL = 'http://localhost:3000/api/auth';

/**
 * Common fetch wrapper for auth requests
 */
async function apiFetch(endpoint, method = 'GET', body = null) {
    const options = {
        method,
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include' // Crucial for HttpOnly cookies
    };

    if (body) {
        options.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(`${BASE_URL}${endpoint}`, options);
        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || 'Произошла ошибка');
        }

        return data;
    } catch (error) {
        console.error(`API Error (${endpoint}):`, error);
        throw error;
    }
}

export const authApi = {
    register: (email, password) => apiFetch('/register', 'POST', { email, password }),
    verify: (email, code) => apiFetch('/verify', 'POST', { email, code }),
    login: (email, password) => apiFetch('/login', 'POST', { email, password }),
    forgotPassword: (email) => apiFetch('/forgot-password', 'POST', { email }),
    resetPassword: (email, code, newPassword) => apiFetch('/reset-password', 'POST', { email, code, newPassword }),
    logout: () => apiFetch('/logout', 'POST'),
    check: () => apiFetch('/check', 'GET'),
    resendCode: (email) => apiFetch('/resend-code', 'POST', { email }),
    getProfile: () => apiFetch('/profile', 'GET'),
    // Вспомогательные методы для произвольных запросов
    get: (endpoint) => apiFetch(endpoint, 'GET'),
    post: (endpoint, body) => apiFetch(endpoint, 'POST', body)
};
