<script>
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';
    function goToProfile(){
      goto(`${base}/profile`)
    }
</script>

<div class="login-container">
  <h1 class="title">Приветствуем!</h1>
  
  <p class="subtitle">Войди чтобы создать свое профессиональное будущее.</p>
  
  <form>
    <div class="form-group">
      <label for="email">АДРЕС ЭЛЕКТРОННОЙ ПОЧТЫ</label>
      <input
        type="email"
        id="email"
        placeholder="example@mail.com"
      />
    </div>
    
    <div class="form-group">
      <label for="password">ПАРОЛЬ</label>
      <input
        type="password"
        id="password"
        placeholder="••••••••"
      />
    </div>
    
    <div class="links">
      <a href="{base}/registration" class="register-link">НЕТ АККАУНТА? СОЗДАЙТЕ ЕГО!</a>
      <a href="{base}/SignIn/restorePassword" class="forgot-link">ЗАБЫЛИ ПАРОЛЬ?</a>
    </div>
    
    <button type="button" on:click={goToProfile} class="submit-btn">
      ВОЙТИ
    </button>
  </form>
</div>

<style>
  .login-container {
    width: 100%;
    max-width: 600px;
    margin: 0 auto;
    padding: 2rem;
    background-color: rgb(226, 239, 247, 1);
    border-radius: 16px;
    box-shadow: 10px 10px 10px 5px rgba(0, 0, 0, 0.1);
  }
  
  .title {
    font-size: 3rem;
    font-weight: 700;
    color: rgba(29, 53, 87, 1);
    margin: 0 0 0.5rem 0;
    text-align: center;
  }
  
  .subtitle {
    font-size: 2rem;
    font-weight: bolder;
    color: rgba(102, 155, 188, 1);
    margin: 0 0 2rem 0;
    text-align: center;
  }
  
  .form-group {
    margin-bottom: 1.5rem;
  }
  
  label {
    display: block;
    font-size: 1.2rem;
    font-weight: 600;
    letter-spacing: 0.05em;
    color: rgba(102, 155, 188, 1);
    margin-bottom: 0.5rem;
    text-transform: uppercase;
  }
  
  input {
    width: 100%;
    padding: 0.75rem 1rem;
    font-size: 1.5rem;
    border: 2px solid #e0e0e0;
    border-radius: 8px;
    box-sizing: border-box;
    background: #fafafa;
  }
  
  input:focus {
    outline: none;
    border-color: #667eea;
    background: white;
  }
  
  .links {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 1rem 0 2rem 0;
  }
  
  .register-link {
    color: rgba(29, 53, 87, 1);
    text-decoration: none;
    font-weight: 600;
    font-size: 1rem;
  }
  
  .register-link:hover {
    text-decoration: underline;
  }
  
  .forgot-link {
    color: rgba(102, 155, 188, 1);
    text-decoration: none;
    font-size: 1rem;
  }
  
  .forgot-link:hover {
    text-decoration: underline;
  }
  
  .submit-btn {
    width: 100%;
    padding: 1rem;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 8px;
    font-size: 1.25rem;
    font-weight: 600;
    letter-spacing: 0.05em;
    cursor: pointer;
    text-transform: uppercase;
  }
  
  .submit-btn:hover {
    background: #5a67d8;
  }
</style>
