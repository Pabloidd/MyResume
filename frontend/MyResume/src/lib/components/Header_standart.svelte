<script>
  import logo from "$lib/images/logo.svg"
  import exit from "$lib/images/exit.svg"

  import { page } from '$app/stores';
  import { base } from '$app/paths';

</script>

<header class="dashboard-header">

      <img src="{logo}" class="logo" alt="MyResume">
        
      <div class="nav-container">
        <nav class="main-nav">
            <a href="{base}/search"  class="nav-link"  class:isActive={$page.url.pathname === '/search'}>Поиск Резюме</a>
            <a href="{base}/profile" class="nav-link " class:isActive={$page.url.pathname === '/profile'}>Личный кабинет</a>
        </nav>

        <button class="logout-btn">
            <img src="{exit}" class="logout-icon" alt="exit"/>
            Выйти
        </button>
    </div>
</header>

<style>
  .dashboard-header {
    background-color: rgba(102, 155, 188, 0.55);
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    padding: 0 2.5rem;
    padding-bottom: 0.75rem;

  }

  .logo{
  margin: 24px 0px 25px 32px;
  max-width: 251px;
  max-height: 66px;
}

.nav-container{
    display: flex;
    flex-direction: row;
    gap: 5rem;
}

  /* Навигация */
  .main-nav {
    display: flex;
    gap: 4rem;
    align-items: center;
  }

  .nav-link {
    color: rgba(255, 255, 255, 1);
    text-decoration: none;
    font-size: 1.75rem;
    font-weight: bold;
    transition: all 0.2s;
  }

  .isActive{
    color: #1f2937;
    font-weight: bolder;
    box-shadow: 0px -5px 3px -5px inset black;
  }

  .nav-link:hover {
    color: #1f2937;
  }

  /* Кнопка выхода */
  .logout-btn {
    background-color: rgba(193, 18, 31, 0.63);
    border-radius: 8px;
    padding: 0.5rem 1rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
    color: rgba(255, 255, 255, 1);
    font-size: 1.25rem;
    cursor: pointer;
    transition: all 0.2s;
  }

  .logout-btn:hover {
    background-color: rgba(255, 0, 17, 0.63);
  }

  .logout-icon {
    font-size: 1.1rem;
  }
/* ===== АДАПТИВНОСТЬ ===== */

/* 1300px */
@media (max-width: 1300px) {
  .dashboard-header {
    padding: 0 2rem 0.75rem 2rem;
  }
  
  .logo {
    max-width: 220px;
    margin-left: 20px;
  }
  
  .nav-container {
    gap: 3rem;
  }
  
  .main-nav {
    gap: 3rem;
  }
  
  .nav-link {
    font-size: 1.5rem;
  }
  
  .logout-btn {
    font-size: 1.1rem;
    padding: 0.5rem 1.2rem;
  }
}

/* 1080px */
@media (max-width: 1080px) {
  .dashboard-header {
    padding: 0 1.5rem 0.75rem 1.5rem;
  }
  
  .logo {
    max-width: 180px;
    margin-left: 15px;
    margin-top: 20px;
    margin-bottom: 20px;
  }
  
  .nav-container {
    gap: 2rem;
  }
  
  .main-nav {
    gap: 2rem;
  }
  
  .nav-link {
    font-size: 1.5rem;
  }
  
  .logout-btn {
    font-size: 1rem;
    padding: 0.4rem 1rem;
  }
}

/* 900px */
@media (max-width: 900px) {
  .dashboard-header {
    flex-direction: column;
    align-items: stretch;
    padding: 0 1.5rem 1rem 1.5rem;
  }
  
  .logo {
    max-width: 200px;
    margin: 15px auto;
  }
  
  .nav-container {
    flex-direction: row;  /* Оставляем в ряд */
    justify-content: center;
    gap: 2rem;
  }
  
  .main-nav {
    gap: 2rem;
  }
  
  .nav-link {
    font-size: 1.5rem;
  }
  
  .logout-btn {
    font-size: 1rem;
    padding: 0.4rem 1rem;
    width: auto;  /* Не растягиваем */
  }
  .logout-icon{
    max-width: 20px;
  }
}

/* 650px */
@media (max-width: 650px) {
  .dashboard-header {
    padding: 0 1rem 0.75rem 1rem;
  }
  
  .logo {
    max-width: 200px;
    margin: 12px auto;
  }
  
  .nav-container {
    gap: 1rem;
    justify-content: center;
  }
  
  .main-nav {
    gap: 1rem;
  }
  
  .nav-link {
    font-size: 1.5rem;
    padding: 0.3rem 0;
  }
  
  .logout-btn {
    font-size: 0; 
    padding: 0.05rem;
  }
  
  .logout-icon {
    width: 20px;
    height: 15px;
  }
  
}

/* 475px */
@media (max-width: 475px) {
  
  .logo {
    margin: 10px auto;
  }
  
  .nav-container {
    flex-direction: column;  /* В колонку на телефонах */
    align-items: center;
    gap: 0.75rem;
  }
  
  .main-nav {
    flex-direction: column;  /* Ссылки тоже в колонку */
    gap: 0.5rem;
    align-items: center;
  }
  
  .nav-link {
    padding: 0.25rem 0;
  }
  
  .logout-btn {
    padding: 0.4rem 1.5rem;
    width: fit-content;
    margin-top: 0.25rem;
  }
  
  .logout-icon {
    width: 16px;
    height: 16px;
  }
}




</style>
