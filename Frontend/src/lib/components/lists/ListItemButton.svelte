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

<button class="btn btn-primary list-item d-flex flex-row justify-content-between align-items-center w-100 text-light l-item" 
        onclick={() => action()}>
    <span class="ms-2">{label}</span>
    <span class="d-flex flex-row justify-content-between align-items-center me-0">
        {#if info !== undefined || subInfo !== undefined}
            <span class="d-flex flex-column align-items-end me-2">
                {#if info !== undefined}
                    <span class="text-info">{info}</span>
                {/if}
                {#if subInfo !== undefined}
                    <span class="text-warning">{subInfo}</span>
                {/if}
            </span>
        {/if}
    </span>
</button>