<script lang="ts">
    import {Input} from "@sveltestrap/sveltestrap";
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    let {
        id,
        label,
        isChecked,
        onchange,
        contextActions = []
    }: {
        id: string,
        label: string,
        isChecked: boolean,
        onchange: () => void,
        contextActions: ContextAction[]
    } = $props()
</script>


<ContextMenuCustom actions={contextActions} id={id} name={label}>
        <Input type="checkbox"
               name={id}
               label={label}
               value={id}
               class="list-item"
               checked={isChecked}
               onchange={onchange}
        />
</ContextMenuCustom>