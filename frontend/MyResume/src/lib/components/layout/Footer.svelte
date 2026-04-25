<footer>
    <p class="footer_text">© 2026 MyResume. Все права защищены.</p>
</footer>


<style>
    footer {
        width: 100%;
        min-height: 72px;
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 1rem 1.25rem;
        background-color: var(--chrome-bg, rgba(102, 155, 188, 0.95));
        backdrop-filter: var(--chrome-blur, blur(10px));
        -webkit-backdrop-filter: var(--chrome-blur, blur(10px));
        box-shadow: 0 -2px 10px rgba(0, 0, 0, 0.06);
    }

    .footer_text {
        margin: 0;
        color: rgba(29, 53, 87, 0.72);
        font-size: clamp(0.8rem, 2.5vw, 1.15rem);
        text-align: center;
        line-height: 1.4;
        max-width: 100%;
        word-wrap: break-word;
    }

    @media (max-width: 475px) {
        footer {
            min-height: 64px;
            padding: 0.75rem 1rem;
        }
    }
</style>