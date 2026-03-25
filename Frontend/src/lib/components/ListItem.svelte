<script lang="ts">
    import { onMount } from 'svelte';
    import ContextMenu, { Item } from "svelte-contextmenu";

    let { 
        name, 
        id,
        link, 
        subtitle, 
        isTop = false, 
        isBottom = false,
        actions = []
    } : {
        name: string,
        id: string,
        link: string,
        subtitle: string,
        isTop: boolean,
        isBottom: boolean,
        actions: ContextAction[]
    } = $props()
    
    let myMenu: ContextMenu | null = $state(null)
    
    let mounted = $derived(false);
    
    onMount(() => {
        mounted = true;
    });
</script>

<li>
    {#if mounted}
        <ContextMenu bind:this={myMenu}>
            {#each actions as action, i}
                <Item 
                        on:click={() => {action.action(id, name)}} 
                        class="{i === 0 ? 'top' : ''} {i === actions.length - 1 ? 'bottom' : ''} {action.isDestructive ? 'delete' : ''} "
                >
                    {action.label}
                </Item>
            {/each}
        </ContextMenu>
    {/if}
    
    <a href="{link}" class="btn btn-primary list-button {isTop ? 'top' : ''} {isBottom ? 'bottom' : ''}" role="button"
       oncontextmenu={(e) => {
           if (!e.shiftKey) {
                myMenu?.show(e);
           }
       }}>
            <div class="d-flex flex-row justify-content-between">
                <span>{name}</span>
                <div class="d-flex flex-row">
                    <span>{subtitle}</span>
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