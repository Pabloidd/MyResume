<script>
    import arrow from "$lib/images/arrow.svg";
    import { goto } from "$app/navigation";

  function goToSignIn(){
    goto('/SignIn')
  }
</script>

<section class="slogan">
    <h1>Профессиональное резюме</h1>
    <h2 class="h"> за 15 минут</h2>
    <h2>Создайте впечатляющее резюме с помощью нашего чистого, минималистичного конструктора. Больше воздуха, меньше - только ваша карьера.</h2>
    <button class="button" on:click={goToSignIn}>Создать резюме сейчас <img class="arrow_img" src={arrow} alt=""></button>
</section>
    <style>
        .slogan{
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            background: linear-gradient(135deg, rgba(255, 255, 255, 1) 0%, rgba(102, 155, 188, 1) 750%);
        }

        h1{
            margin: 25px 272px 80px;
            color: rgba(29, 53, 87, 1);
            font-weight: bold;
            font-size: 72px;
        }
        .h{
            margin: -75px 272px 80px;
            color: rgba(102, 155, 188, 1);
            font-weight: bold;
            font-size: 72px;
        }

        h2{
            margin: -50px 272px 80px;
            color: rgba(29, 53, 87, 0.7);
            font-size: 25px;
            font-weight: lighter;
        }

        .button{
            margin: -30px 272px 80px;
            padding: 35px 50px 35px 50px;
            background-color: rgba(102, 155, 188, 1);
            color: rgba(255, 255, 255, 1);
            display: flex;
            align-items: center;
            max-width: 400px;
            max-height: 56px;
            border-radius: 9999px;
            font-size: 22px;
            white-space: nowrap ;
        }
        
        .arrow_img{
            max-height: 20px;
            max-width: 20px;
        }
    </style>