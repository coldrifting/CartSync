<script lang="ts">
    import {fade} from 'svelte/transition';
    import {Spinner} from "@sveltestrap/sveltestrap";
</script>

<style>
    .loading-page {
        min-height: 90dvh;
    }
    
    .loading-page-spinner {
        width: 6rem;
        height: 6rem;
        border: 1rem solid #e4115a;
        border-top: 1rem solid transparent;
        border-radius: 50%;
        animation: spin 1s linear infinite;
    }

    @keyframes spin {
        0% {
            transform: rotate(0deg);
        }
        100% {
            transform: rotate(360deg);
        }
    }
</style>

<div class="d-flex flex-column align-items-center justify-content-center w-100 h-100 loading-page" 
     in:fade={{duration: 200, delay: 400}}>
    <div class="loading-page-spinner">
        
    </div>
</div>