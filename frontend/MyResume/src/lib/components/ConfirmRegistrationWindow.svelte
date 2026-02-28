<script>
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';

    function goToMain(){
        goto('{base}/')
    }
</script>

<div class="verify-container">
  <h1 class="title">РЕГИСТРАЦИЯ</h1>
  
  <p class="message">Код подтверждения отправлен на вашу электронную почту</p>
  
  <form>
    <div class="form-group">
      <label for="code">Введите код</label>
      <input
        type="text"
        id="code"
        placeholder="••••••"
      />
    </div>
    
    <button on:click={goToMain} type="button" class="submit-btn">
      ЗАРЕГИСТРИРОВАТЬСЯ
    </button>
  </form>
</div>

<style>
  .verify-container {
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
    margin: 0 0 1rem 0;
    text-align: center;
  }
  
  .message {
    font-size: 1.5rem;
    color: rgba(102, 155, 188, 1);
    margin: 0 0 2rem 0;
    text-align: center;
    line-height: 1.4;
  }
  
  .form-group {
    margin-bottom: 2rem;
  }
  
  label {
    display: block;
    font-size: 1.2rem;
    font-weight: 600;
    letter-spacing: 0.05em;
    color: rgba(102, 155, 188, 1);
    margin-bottom: 0.5rem;
  }
  
  input {
    width: 100%;
    padding: 0.75rem 1rem;
    font-size: 1.5rem;
    border: 2px solid #e0e0e0;
    border-radius: 8px;
    box-sizing: border-box;
    background: #fafafa;
    text-align: center;
    letter-spacing: 0.5rem;
  }
  
  input:focus {
    outline: none;
    border-color: #667eea;
    background: white;
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
