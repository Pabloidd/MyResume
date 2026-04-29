import { writable } from 'svelte/store';

/** Счётчик активных сетевых операций (для глобального прогресс-бара) */
export const remoteLoading = writable(0);

export function beginRemoteLoad() {
  remoteLoading.update((n) => n + 1);
}

export function endRemoteLoad() {
  remoteLoading.update((n) => Math.max(0, n - 1));
}
