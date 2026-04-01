<script lang="ts">
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    interface Props {
        id: string;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        contextActions?: ContextAction[] | undefined;
        link: string;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        contextActions = undefined,
        link
    }: Props = $props()
</script>

<ContextMenuCustom contextActions={contextActions} id={id} name={label}>
    <a href="{link}" class="btn btn-primary list-item" role="button">
        <div class="d-flex flex-row justify-content-between align-items-center">
            <span>{label}</span>
            <div class="d-flex flex-row justify-content-between align-items-center">
                {#if info !== undefined || subInfo !== undefined}
                    <div class="d-flex flex-column">
                        {#if info !== undefined}
                            <span class="info {info.startsWith('(') ? 'info-secondary' : ''}">{info}</span>
                        {/if}
                        {#if subInfo !== undefined}
                            <span class="info-sub">{subInfo}</span>
                        {/if}
                    </div>
                {/if}
                <svg aria-hidden="true"
                     xmlns="http://www.w3.org/2000/svg"
                     width="16"
                     height="16"
                     fill="none"
                     viewBox="0 0 16 16">
                    <path stroke="strokeColor" stroke-linecap="round" stroke-width="1.5"
                          d="M8,0 16,8 8,16"/>
                </svg>
            </div>
        </div>
    </a>
</ContextMenuCustom>