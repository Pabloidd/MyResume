<script>
  import { navigating } from '$app/stores';
  import { remoteLoading } from '$lib/stores/remoteLoading';

  $: active = !!$navigating || $remoteLoading > 0;
</script>

<div class="bar-wrap" class:active aria-hidden={!active}>
  <div class="bar" />
</div>

<style>
  .bar-wrap {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    height: 3px;
    z-index: 100000;
    pointer-events: none;
    overflow: hidden;
    opacity: 0;
    transition: opacity 0.2s ease;
  }

  .bar-wrap.active {
    opacity: 1;
  }

  .bar {
    height: 100%;
    width: 40%;
    background: linear-gradient(
      90deg,
      rgba(102, 155, 188, 0.15) 0%,
      rgba(102, 155, 188, 1) 45%,
      rgba(29, 53, 87, 1) 55%,
      rgba(102, 155, 188, 0.15) 100%
    );
    background-size: 200% 100%;
    animation: slide 0.9s ease-in-out infinite;
    border-radius: 0 2px 2px 0;
  }

  @keyframes slide {
    0% {
      transform: translateX(-100%);
    }
    100% {
      transform: translateX(350%);
    }
  }
</style>
