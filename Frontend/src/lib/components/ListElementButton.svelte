<script lang="ts">
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    interface Props {
        id: string;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        contextActions?: ContextAction[] | undefined;
        action: () => void;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        contextActions = undefined,
        action
    }: Props = $props()
</script>

<ContextMenuCustom contextActions={contextActions} id={id} name={label}>
    <button class="btn btn-primary list-item d-flex flex-row justify-content-between align-items-center"
         onclick={() => action()}
    >
        <span>{label}</span>
        <span class="d-flex flex-row justify-content-between align-items-center">
            {#if info !== undefined || subInfo !== undefined}
                <span class="d-flex flex-column">
                    {#if info !== undefined}
                        <span class="info {info.startsWith('(') ? 'info-secondary' : ''}">{info}</span>
                    {/if}
                    {#if subInfo !== undefined}
                        <span class="info-sub">{subInfo}</span>
                    {/if}
                </span>
            {/if}
        </span>
    </button>
</ContextMenuCustom>