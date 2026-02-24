<script>
  import logo from "$lib/images/logo.svg"
  import { goto } from "$app/navigation";

  function goToSignIn(){
    goto('/SignIn')
  }
</script>

<header class="main_page_header">
  <img src={logo} class="logo" alt="MyResume_logo">
  <div class="registration_container">
    <a href="/SignIn">Войти</a>
  </div>
</header>

<style>
.main_page_header{
  background-color: rgba(102, 155, 188, 0.55);

  display: flex;
  flex-direction: row;
  justify-content: space-between;

  max-height: 115px;
}

.registration_container{
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 22px;
  margin: 39px 32px 40px 0px;
}

.registration_container a{
  font-size: 25px;
  font-weight: bold;
  text-decoration: none;
  color: rgba(29, 53, 87, 1);
}

.logo{
  margin: 24px 0px 25px 32px;
  max-width: 251px;
  max-height: 66px;
}

</style>