<script>
  import { onMount } from 'svelte';
  import { auth } from '$lib/authStore';
  import { authApi } from '$lib/authApi';
  import { goto } from '$app/navigation';
  import { base } from '$app/paths';
  import toast from 'svelte-french-toast';

  let users = [];
  let isLoading = true;
  let searchTerm = '';

  $: filteredUsers = users.filter(user => 
    user.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  async function fetchUsers() {
    isLoading = true;
    try {
      const response = await authApi.get('/admin/users');
      if (response.success) {
        users = response.users;
      } else {
        toast.error('Ошибка загрузки пользователей');
      }
    } catch (error) {
      console.error('Error fetching users:', error);
      toast.error('Ошибка сервера при загрузке пользователей');
    } finally {
      isLoading = false;
    }
  }

  async function handleRoleChange(userId, newRole) {
    try {
      const response = await authApi.post('/admin/change-role', {
        userId,
        newRole
      });
      
      if (response.success) {
        toast.success('Роль успешно изменена');
        // Обновляем локальный список
        users = users.map(u => u._id === userId ? { ...u, role: newRole } : u);
      } else {
        toast.error(response.error?.message || 'Ошибка при смене роли');
      }
    } catch (error) {
       toast.error('Ошибка сервера');
    }
  }

  onMount(() => {
    if (!$auth.isAuthenticated || $auth.user?.role !== 'admin') {
      goto(`${base}/profile`);
      return;
    }
    fetchUsers();
  });
</script>

<div class="admin-panel">
  <div class="admin-header">
    <button class="back-btn" on:click={() => goto(`${base}/profile`)}>← К профилю</button>
    <h1 class="admin-title">Управление пользователями</h1>
  </div>

  <div class="admin-controls">
    <div class="search-box">
      <input 
        type="text" 
        placeholder="Поиск по email..." 
        bind:value={searchTerm}
        class="search-input"
      />
    </div>
  </div>

  <div class="users-table-container">
    {#if isLoading}
      <div class="loading-state">
        <div class="spinner"></div>
        <p>Загрузка данных...</p>
      </div>
    {:else if filteredUsers.length === 0}
      <div class="empty-state">
        <p>Пользователи не найдены</p>
      </div>
    {:else}
      <table class="users-table">
        <thead>
          <tr>
            <th>Email</th>
            <th>Текущая роль</th>
            <th>Изменить роль</th>
          </tr>
        </thead>
        <tbody>
          {#each filteredUsers as user}
            <tr>
              <td class="user-email">{user.email}</td>
              <td class="user-role-cell">
                <span class="role-badge" class:role-admin={user.role === 'admin'} class:role-premium={user.role === 'premium'}>
                  {user.role}
                </span>
              </td>
              <td>
                <select 
                  class="role-select" 
                  value={user.role} 
                  on:change={(e) => handleRoleChange(user._id, e.target.value)}
                  disabled={user.email === $auth.user?.email}
                >
                  <option value="user">User (Стандарт)</option>
                  <option value="premium">Premium</option>
                  <option value="admin">Admin</option>
                </select>
              </td>
            </tr>
          {/each}
        </tbody>
      </table>
    {/if}
  </div>
</div>

<style>
  .admin-panel {
    max-width: 1200px;
    margin: 2rem auto;
    padding: 2rem;
    background: #ffffff;
    border-radius: 20px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.05);
  }

  .admin-header {
    display: flex;
    align-items: center;
    gap: 2rem;
    margin-bottom: 2rem;
  }

  .back-btn {
    background: #f1f5f9;
    border: none;
    padding: 0.5rem 1rem;
    border-radius: 8px;
    cursor: pointer;
    font-weight: 500;
    transition: all 0.2s;
  }

  .back-btn:hover {
    background: #e2e8f0;
  }

  .admin-title {
    font-size: 2rem;
    font-weight: 800;
    color: #1e293b;
    margin: 0;
  }

  .admin-controls {
    margin-bottom: 2rem;
  }

  .search-input {
    width: 100%;
    max-width: 400px;
    padding: 0.75rem 1rem;
    border: 1px solid #e2e8f0;
    border-radius: 12px;
    font-size: 1rem;
    outline: none;
    transition: border-color 0.2s;
  }

  .search-input:focus {
    border-color: #2563eb;
  }

  .users-table-container {
    overflow-x: auto;
  }

  .users-table {
    width: 100%;
    border-collapse: collapse;
    text-align: left;
  }

  .users-table th {
    padding: 1rem;
    border-bottom: 2px solid #f1f5f9;
    color: #64748b;
    font-weight: 600;
    text-transform: uppercase;
    font-size: 0.85rem;
  }

  .users-table td {
    padding: 1rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: middle;
  }

  .user-email {
    font-weight: 500;
    color: #1e293b;
  }

  .role-badge {
    padding: 0.25rem 0.75rem;
    border-radius: 999px;
    font-size: 0.85rem;
    font-weight: 600;
    background: #f1f5f9;
    color: #64748b;
  }

  .role-admin {
    background: #fee2e2;
    color: #ef4444;
  }

  .role-premium {
    background: #fef3c7;
    color: #d97706;
  }

  .role-select {
    padding: 0.5rem;
    border-radius: 8px;
    border: 1px solid #e2e8f0;
    outline: none;
    background: #fff;
    cursor: pointer;
  }

  .status-indicator {
    font-size: 0.85rem;
    color: #94a3b8;
  }

  .status-verified {
    color: #10b981;
  }

  .loading-state, .empty-state {
    text-align: center;
    padding: 4rem;
  }

  .spinner {
    width: 40px;
    height: 40px;
    border: 3px solid #f1f5f9;
    border-top-color: #2563eb;
    border-radius: 50%;
    margin: 0 auto 1rem;
    animation: spin 1s linear infinite;
  }

  @keyframes spin {
    to { transform: rotate(360deg); }
  }
</style>
