<script>
    export let header_text;
    export let text;
    export let img_src;
    export let img_alt;
</script>

<section class="feature_card">
    <h4 class="card--header_text">{header_text}</h4>
    <p class="card--text">{text}</p>
    <div class="img_container">
        <img class="card--img" src={img_src} alt={img_alt}>
    </div>
</section>

<style>
    .feature_card{
        border-radius: 16px;
        background-color: rgba(241, 250, 238, 1);
        max-width: 352px;
        padding: 0 20px;
        padding-bottom: 15px;
        text-align: justify;
        
    }

    .card--header_text{
        font-size: 25px;
        font-weight: bold;
    }

    .card--text{
        font-size: 22px;
    }

    .img_container{
        display: flex;
        flex-direction: row;
        justify-content: flex-end;
    }

    .card--img{
        max-width: 56px;
        max-height: 56px;
    }

    @media(max-width: 1300px){
       .card--img{
        max-width: 46px;
        max-height: 46px;
    } 
    .feature_card{
        max-width: 250px;
        padding: 0 20px;
        
    }
    }
    @media(max-width: 1075px){
    .card--header_text{
        font-size: 23px;
    }

    .card--text{
        font-size: 20px;
    }
    .feature_card{
        max-width: 200px;
        
    }
}

@media(max-width: 900px){
       .card--img{
        max-width: 40px;
    } 
    .card--header_text{
        font-size: 25px;
    }

    .card--text{
        font-size: 20px;
    }
    .feature_card{
        max-width: 350px;
        padding: 25px 25px;

        
    }
}

@media(max-width: 475px){
       .card--img{
        max-width: 40px;
    } 
    .card--header_text{
        font-size: 25px;
    }

    .card--text{
        font-size: 22px;
    }
    .feature_card{
        max-width: 200px;
        padding: 5px 15px;

        
    }
}
</style>