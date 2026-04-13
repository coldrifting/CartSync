<script lang="ts">
    interface Props {
        children: any;
        onclick: () => void;
    }
    
    let {children, onclick}: Props = $props();
</script>

<style>
    .heading-button {
        background: none;
        border: none;
        color: var(--theme-primary);
        font-size: 1.5rem;
        padding: 0;
        margin-top: 1rem;
        margin-bottom: 0.25rem;
        
        &:hover {
            color: var(--theme-primary-hover);
        }
        
        &:active {
            color: var(--theme-primary-active)
        }
    }
</style>

<button class="heading-button" onclick={onclick}>
    {@render children()}
</button>