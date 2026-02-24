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
</style>