<script>
  import logo from "$lib/images/logo.svg"
  
</script>

<header class="main_page_header">
  <img src={logo} class="logo" alt="MyResume_logo">
    <a class="a" href="/SignIn">Войти</a>
</header>

<style>
.main_page_header{
  background-color: rgba(102, 155, 188, 0.55);

  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  padding: 0 2.5rem;
  max-height: 115px;
}

.a{
  margin: 39px 4rem 40px 0;
  font-size: 25px;
  font-weight: bold;
  text-decoration: none;
  color: rgba(29, 53, 87, 1);
  padding: 0.25rem;
  box-shadow: 0px -5px 3px -4px inset black;
  transition: 0.25s;
}


.a:hover{
  color: rgb(57, 95, 149);
  
}

.logo{
  margin: 24px 0px 25px 32px;
  max-width: 251px;
  max-height: 66px;
}

@media (max-width: 900px){
  .a{
    font-size: 22px;
  }
  .logo{
  margin: 20px 0px 25px 18px;
  max-width: 200px;
}
}

@media (max-width: 650px){
  .a{
    margin: 39px 0 40px ;
    font-size: 20px;
  }
  .logo{
  max-width: 200px;
}
}

@media (max-width: 475px){
  .a{
    font-size: 18px;
  }
  .logo{
  
  max-width: 125px;
}
.main_page_header{
  padding: 0 0.75rem 0rem 0 ;
  max-height: 75px;
}
}
</style>