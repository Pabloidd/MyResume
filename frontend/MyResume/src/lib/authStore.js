import { writable } from 'svelte/store';

function createAuthStore() {
    const { subscribe, set, update } = writable({
        user: null,
        isAuthenticated: false,
        isLoading: true
    });

    return {
        subscribe,
        setUser: (user) => set({ user, isAuthenticated: !!user, isLoading: false }),
        setLoading: (isLoading) => update(s => ({ ...s, isLoading })),
        logout: () => set({ user: null, isAuthenticated: false, isLoading: false })
    };
}

export const auth = createAuthStore();
