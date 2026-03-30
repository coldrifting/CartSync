<script lang="ts">
    import {onMount} from 'svelte';
    import ContextMenu, {Item} from "svelte-contextmenu";
    import {Input} from "@sveltestrap/sveltestrap";

    let {
        name,
        value,
        label,
        group,
        onchange,
        actions = []
    }: {
        name: string,
        value: string,
        label: string,
        group: string,
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
        <!-- Prevent deletion of current item -->
        {#each actions.filter(a => !a.isDestructive || value !== group) as action}
            <Item on:click={() => {action.action(value, label)}} class={action.isDestructive ? 'delete' : ''}>
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
    <Input type="radio"
           name={name}
           label={label}
           value={value}
           bind:group={group}
           onchange={onchange}
    />
</li>