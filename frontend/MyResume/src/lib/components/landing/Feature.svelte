<script>
	import FeatureCard from './FeatureCard.svelte';
    import heart from '$lib/images/heart.svg';
    import mishen from '$lib/images/mishen.svg';
    import flesh from '$lib/images/flesh.svg';
</script>
    
<article class="features">  
    <h3 class="first_h">Почему выбирают нас?</h3>
    <h3 class="second_h">Мы убрали всё лишнее, оставили только главное</h3>
    <div class="cards_container">
        <FeatureCard header_text="Молниеносно" text="Заполните простую анкету, и алгоритм сам соберет идеальную структуру. Никакой возни с отступами и шрифтами." img_src={flesh} img_alt="иконка молнии" />
        <FeatureCard header_text="Точно в цель" text="Наши шаблоны разработаны с учетом требований HR-специалистов ведущих компаний. Спокойные тона и четкая структура." img_src={mishen} img_alt="иконка мишени" />
        <FeatureCard header_text="Дружелюбие" text="Автоматические системы проверки резюме легко считывают наши PDF файлы. Шансы на собеседование выше." img_src={heart} img_alt="иконка сердечка" />
    </div>
</article>

<style>
    .features{
        display: flex;
        flex-direction: column;
        align-items: center;
        text-align: center;
        background-color: rgba(255, 255, 255, 1);
    }

    .first_h{
        margin: 96px 0 0 0;
        font-size: 36px;
        font-weight: bold;
        color: rgba(29, 53, 87, 1);
    }

    .second_h{
        font-size: 22px;
        
        color: rgba(102, 155, 188, 1);
    }
    .cards_container{
        margin-top: 25px;
        margin-bottom: 96px;
        display: flex;
        flex-direction: row;
        gap: 42px;
    }


    @media(max-width: 900px){
        .cards_container{
            flex-direction: column;
    }
    .first_h{
        margin: 18px 0 0 0;
        font-size: 27px;

    }

    .second_h{
        font-size: 22px;
    }
    }   
         @media(max-width: 475px){

    .first_h{
        margin: 18px 0 0 0;
        font-size: 25px;

    }

    .second_h{
        font-size: 20px;
    }
    } 
    
</style>