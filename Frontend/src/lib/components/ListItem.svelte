<script lang="ts">
    import { onMount } from 'svelte';
    import ContextMenu, { Item, Settings } from "svelte-contextmenu";

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
    
    const boldSettings = Settings.BootstrapCss();
    
    let mounted = $derived(false);
    
    onMount(() => {
        mounted = true;
    });
</script>

<li>
    {#if mounted}
        <ContextMenu bind:this={myMenu} settings={boldSettings} >
            {#each actions as action}
                <Item on:click={() => {action.action(id, name)}} class={action.isDestructive ? 'text-danger hover:bg-red-500/50 hover:text-white' : ''} >
                    {action.label}
                </Item>
            {/each}
        </ContextMenu>
    {/if}
    
    <a href="{link}" class="btn btn-primary list-button {isTop && isBottom 
                                                        ? 'list-button-single' 
                                                        : isTop 
                                                            ? 'list-button-top' 
                                                            : isBottom 
                                                                ? 'list-button-bottom' 
                                                                : ''}" role="button"
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