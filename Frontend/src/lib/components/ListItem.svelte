<script lang="ts">
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    interface Props {
        name: string,
        id: string,
        link: string,
        subtitle?: string | undefined,
        contextActions: ContextAction[]
    }
    
    let {name, id, link, subtitle = undefined, contextActions = []}: Props = $props()
</script>

<ContextMenuCustom actions={contextActions} id={id} name={name}>
    <a href="{link}" class="btn btn-primary list-item" role="button">
        <div class="d-flex flex-row justify-content-between">
            <span>{name}</span>
            <div class="d-flex flex-row">
                <span class={subtitle && !Number.isInteger(subtitle) && !(subtitle.startsWith('(')) ? '' : 'text-secondary'}>{subtitle ?? ""}</span>
                <svg class="mt-1"
                     aria-hidden="true"
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