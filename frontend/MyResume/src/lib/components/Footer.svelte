<footer>
    <p class="footer_text">© 2026 MyResume. Все права защищены.</p>
</footer>


<style>
    footer{
        max-height: 84px;
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: rgba(255, 255, 255, 1);
    }
    .footer_text{
        
        color: rgba(29, 53, 87, 0.5);
        font-size: 20px;
        white-space: nowrap;    /*запрет переноса, убрать для мабилок*/
    }

    @media(max-width: 475px){
        .footer_text{
            font-size: 15px;
        }
    }
</style>