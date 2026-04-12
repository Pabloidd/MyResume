<script>
    import arrow from "$lib/images/arrow.svg";
    import { goto } from "$app/navigation";
    import { base } from '$app/paths';
    import { auth } from "$lib/authStore";

    function goToSignIn() {
        if ($auth.isAuthenticated) {
            goto(`${base}/profile`);
        } else {
            goto(`${base}/SignIn`);
        }
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
            transition: 0.25s;
        }

        .button:hover{
            transform: scale(1.05);
        }
        
        .arrow_img{
            max-height: 20px;
            max-width: 20px;
        }

        @media (max-width: 1080px){
            h1{
            margin: 25px 152px 80px;
            color: rgba(29, 53, 87, 1);
            font-weight: bold;
            font-size: 50px;
        }
        .h{
            margin: -75px 152px 80px;
            color: rgba(102, 155, 188, 1);
            font-weight: bold;
            font-size: 50px;
        }

        h2{
            margin: -50px 152px 80px;
            color: rgba(29, 53, 87, 0.7);
            font-size: 25px;
            font-weight: lighter;
        }
        .button{
            margin: -40px 150px 60px;
            padding: 25px 35px 25px 35px;
            max-width: 350px;
            max-height: 46px;
            font-size: 17px;
        }
        }
        @media (max-width: 900px){
            h1{
            margin: 25px 102px 80px;
            color: rgba(29, 53, 87, 1);
            font-weight: bold;
            font-size: 35px;
        }
        .h{
            margin: -75px 122px 80px;
            color: rgba(102, 155, 188, 1);
            font-weight: bold;
            font-size: 35px;
        }

        h2{
            margin: -50px 122px 80px;
            color: rgba(29, 53, 87, 0.7);
            font-size: 22px;
            font-weight: lighter;
        }
        .button{
            margin: -50px 122px 80px;
            max-width: 350px;
            max-height: 46px;
            font-size: 17px;
        }
        }
        @media (max-width: 650px){
            h1{
            margin: 25px 52px 80px;
            font-size: 27px;
        }
        .h{
            margin: -75px 52px 80px;
            font-size: 27px;
        }

        h2{
            margin: -70px 52px 80px;
            color: rgba(29, 53, 87, 0.7);
            font-size: 20px;
            font-weight: lighter;
        }
        .button{
            margin: -50px 52px 35px;
            padding: 15px 25px 15px 25px;
            max-width: 250px;
            max-height: 46px;
            font-size: 14px;
        }
        }
        @media (max-width: 475px){
            h1{
            margin: 25px 52px 80px;
            color: rgba(29, 53, 87, 1);
            font-weight: bold;
            font-size: 25px;
        }
        .h{
            margin: -75px 52px 80px;
            color: rgba(102, 155, 188, 1);
            font-weight: bold;
            font-size: 25px;
        }

        h2{
            margin: -75px 52px 80px;
            font-size: 18px;
        }
        .button{
            margin: -60px 52px 25px;
            padding: 5px 5px 5px 5px;
            max-height: 25px;
            font-size: 14px;
        }
        }
    </style>
