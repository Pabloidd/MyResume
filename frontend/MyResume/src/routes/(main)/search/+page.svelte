<script>
    import SearchPage from '$lib/components/search/SearchPage.svelte';
    import { goto } from '$app/navigation';
    import feedback from '$lib/images/feedback.png';
    
    function goToFeedback(){
        goto('/profile/feedback/')
    }
</script>
<SearchPage />
<img class="feedback-img" on:click={goToFeedback} src={feedback} alt="feedback">

<style>
	.feedback-img{
		position: fixed;
		bottom: 1rem;
		right: 1rem;
		max-width: 150px;
	}

    @media(max-width: 1100px){
        .feedback-img{
		bottom: 0.5rem;
		right: 0.5rem;
		max-width: 100px;
	}
    }
    @media(max-width: 500px){
        .feedback-img{
		bottom: 0.25rem;
		right: 0.25rem;
		max-width: 75px;
	}
    }
</style>

