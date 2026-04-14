<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";

    interface Props {
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        action: () => void;
    }

    let {
        label,
        info = undefined,
        subInfo = undefined,
        action
    }: Props = $props()
</script>

<style>
    button {
        margin: 0.25rem 0.75rem;
        width: 100%;
        height: 2.5rem;
        background: none;
        border: none;
        color: var(--theme-fg);
        border-radius: 0.375rem;
        
        .text-info {
            line-height: 1.5rem;
        }
        
        .text-warning {
            line-height: 0.7rem;
            font-size: 0.7rem;
        }
    
        &:focus-visible {
            outline: 0.25rem solid rgba(var(--bs-info-rgb), 0.5);
        }
    }
</style>

<ListItem>
    <button class="d-flex flex-row align-items-center w-100 overflow-hidden text-nowrap" onclick={action}>
        <span class="me-auto truncate">{label}</span>
        <span class="d-flex flex-column align-items-center ms-2">
            {#if info !== undefined}
                <span class="text-info">{info}</span>
            {/if}
            {#if subInfo !== undefined}
                <span class="text-warning">{subInfo}</span>
            {/if}
        </span>
    </button>
</ListItem>