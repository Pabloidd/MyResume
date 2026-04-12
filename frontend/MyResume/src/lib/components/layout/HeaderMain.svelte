<script>
  import logo from "$lib/images/logo.svg"
  import { base } from '$app/paths';
  import { auth } from '$lib/authStore';
  import { authApi } from '$lib/authApi';
  import { goto } from '$app/navigation';
  import toast from 'svelte-french-toast';

  async function handleLogout() {
    try {
      await authApi.logout();
      auth.logout();
      toast.success('Вы вышли из системы');
      goto(`${base}/`);
    } catch (error) {
      toast.error('Ошибка при выходе');
    }
  }
</script>

<header class="main_page_header">
  <a href="{base}/" class="logo-link">
    <img src={logo} class="logo" alt="MyResume_logo">
  </a>
  
  <div class="nav-links">
    {#if $auth.isAuthenticated}
      {#if $auth.user?.role !== 'user'}
        <a class="nav-link" href="{base}/search">Поиск резюме</a>
      {/if}
      <a class="nav-link" href="{base}/profile">Личный кабинет</a>
      <button class="logout-link" on:click={() => { if(confirm('Вы уверены, что хотите выйти?')) handleLogout(); }}>Выйти</button>
    {:else}
      <a class="nav-link" href="{base}/SignIn">Войти</a>
    {/if}
  </div>
</header>

<style>
.main_page_header {
  background-color: rgba(102, 155, 188, 0.95);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 5%;
  min-height: 80px;
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  transition: all 0.3s ease;
}

.logo-link {
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

.logo {
  max-width: 220px;
  height: auto;
  transition: transform 0.2s;
}

.logo:hover {
  transform: scale(1.02);
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 2.5rem;
  flex-wrap: wrap;
  justify-content: flex-end;
}

.nav-link, .logout-link {
  font-family: var(--font-heading);
  font-size: 1.2rem;
  font-weight: 600;
  text-decoration: none;
  color: #1d3557;
  padding: 0.5rem 0.25rem;
  position: relative;
  transition: all 0.3s ease;
  background: none;
  border: none;
  cursor: pointer;
  white-space: nowrap;
  letter-spacing: -0.01em;
}

.nav-link::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 2px;
  background: #1d3557;
  transition: width 0.3s ease;
}

.nav-link:hover::after {
  width: 100%;
}

.nav-link:hover, .logout-link:hover {
  color: #457b9d;
}

.logout-link {
  color: #c1121f;
  background: rgba(193, 18, 31, 0.1);
  padding: 0.5rem 1rem;
  border-radius: 8px;
}

.logout-link:hover {
  background: rgba(193, 18, 31, 0.2);
  color: #c1121f;
}

/* ===== АДАПТИВНОСТЬ ===== */

@media (max-width: 1024px) {
  .nav-links {
    gap: 1.5rem;
  }
  .nav-link, .logout-link {
    font-size: 1.1rem;
  }
  .logo {
    max-width: 180px;
  }
}

@media (max-width: 768px) {
  .main_page_header {
    padding: 1rem 3%;
    flex-direction: column;
    gap: 1rem;
  }
  .nav-links {
    justify-content: center;
    gap: 1rem;
    width: 100%;
  }
  .logo {
    max-width: 160px;
  }
}

@media (max-width: 480px) {
  .nav-links {
    flex-direction: column;
    gap: 0.5rem;
  }
  .nav-link, .logout-link {
    font-size: 1.2rem;
    width: 100%;
    text-align: center;
    padding: 0.75rem;
  }
  .main_page_header {
    padding: 1rem;
  }
}
</style>
