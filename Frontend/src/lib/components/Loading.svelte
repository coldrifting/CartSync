<script lang="ts">
    import {fade} from 'svelte/transition';
    import {Spinner} from "@sveltestrap/sveltestrap";

    interface Props {
        isLoading?: boolean;
    }
    
    let {isLoading = $bindable(undefined)}: Props = $props();
</script>

<style>
    .spinner-container {
        height: 3rem;
    }
</style>

<div class="d-flex flex-column align-items-center justify-content-center w-100">
    <div class="spinner-container">
        {#if isLoading !== false}
            <div in:fade={{duration: 100, delay: 500}}>
                <Spinner/>
            </div>
        {/if}
    </div>
</div>