<script lang="ts">
    import { onMount } from 'svelte';
    import ContextMenu, { Item } from "svelte-contextmenu";

    let { 
        name, 
        id,
        link, 
        subtitle,
        actions = []
    } : {
        name: string,
        id: string,
        link: string,
        subtitle: string,
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
            <Item on:click={() => {action.action(id, name)}} class={action.isDestructive ? 'delete' : ''}>
                {action.label}
            </Item>
        {/each}
    </ContextMenu>
{/if}

<li>
    <a href="{link}" class="btn btn-primary list-button" role="button"
       oncontextmenu={(e) => {
           if (!e.shiftKey) {
                myMenu?.show(e);
           }
       }}>
            <div class="d-flex flex-row justify-content-between">
                <span>{name}</span>
                <div class="d-flex flex-row">
                    <span class='text-secondary'>{subtitle}</span>
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
</li>