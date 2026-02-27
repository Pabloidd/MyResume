<script>
	import ProfilePage from '../../../lib/components/profile/ProfilePage.svelte';
	import { goto } from '$app/navigation';
    import feedback from '$lib/images/feedback.png';
    
    function goToFeedback(){
        goto('/profile/feedback/')
    }

</script>

<ProfilePage />
<img class="feedback-img" on:click={goToFeedback} src={feedback} alt="feedback">

<style>
	.feedback-img{
		position: fixed;
		bottom: 1rem;
		right: 1rem;
		max-width: 150px;
	}
</style>

