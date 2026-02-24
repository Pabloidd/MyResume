<script>
    import { goto } from "$app/navigation";

  function goToSignIn(){
    goto('/SignIn')
  }
</script>

<section class="call_to_action_section">
    <div class="text_container">
        <h4 class="call_to_action_header">Готовы начать карьеру?</h4>
        <p class="call_to_action_text">Присоединяйтесь к тысячам профессионалов, которые уже нашли работу мечты с нашим сервисом.</p>
        <button class="call_to_action_button" on:click={goToSignIn}>Создать резюме бесплатно</button>
    </div>
</section>

<style>
    .call_to_action_section{
        background-color: rgba(29, 53, 87, 1);
    }
    .text_container{
        padding: 96px 336px;
        display: flex;
        flex-direction: column;
        align-items: center;
        text-align: center;
    }
    .call_to_action_header{
        font-size: 36px;
        font-weight: bold;
        color: rgba(255, 255, 255, 1);
    }

    .call_to_action_text{
        font-size: 22px;
        color: rgba(241, 250, 238, 0.8);
        margin-top: -25px;
    }

    .call_to_action_button{
        padding: 18px 29px;
        margin-top: 25px;
        color: rgba(29, 53, 87, 1);
        background-color: rgba(241, 250, 238, 1);
        border-radius: 9999px;
        font-weight: bold;
        font-size: 22px;

    }
</style>