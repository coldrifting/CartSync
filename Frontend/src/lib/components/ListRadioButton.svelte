<script lang="ts">
    import {Input} from "@sveltestrap/sveltestrap";
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    let {
        name,
        value,
        label,
        group,
        onchange,
        contextActions = []
    }: {
        name: string,
        value: string,
        label: string,
        group: string,
        onchange: () => void,
        contextActions: ContextAction[]
    } = $props()
</script>

<ContextMenuCustom actions={contextActions} id={value} name={label}>
    <Input type="radio"
           class="list-item"
           name={name}
           label={label}
           value={value}
           bind:group={group}
           onchange={onchange}/>
</ContextMenuCustom>