<script lang="ts">
    import {onMount} from 'svelte';
    import ContextMenu, {Item} from "svelte-contextmenu";
    import {Input} from "@sveltestrap/sveltestrap";

    let {
        id,
        label,
        isChecked,
        onchange,
        actions = []
    }: {
        id: string,
        label: string,
        isChecked: boolean,
        onchange: () => void,
        actions: ContextAction[]
    } = $props()

    let myMenu: ContextMenu | null = $state(null)

    let mounted = $derived(false);

    onMount(() => {
        mounted = true;
    });
</script>

{#if mounted}
    <ContextMenu bind:this={myMenu}>
        {#each actions as action}
            <Item on:click={() => {action.action(id, label)}} class={action.isDestructive ? 'delete' : ''}>
                {action.label}
            </Item>
        {/each}
    </ContextMenu>
{/if}

<li oncontextmenu={(e) => {
                   if (!e.shiftKey) {
                        myMenu?.show(e);
                   }
               }}>
    <Input type="checkbox"
           name={id}
           label={label}
           value={id}
           checked={isChecked}
           onchange={onchange}
    />
</li>