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
    
    let myMenu: ContextMenu
    
    const boldSettings = Settings.DefaultCss();
        
    boldSettings.Item.Class = [];
    boldSettings.Item.Class.push('w-full');
    boldSettings.Item.Class.push('p-2 pl-5 pr-5');
    boldSettings.Item.Class.push('text-left');
    boldSettings.Item.Class.push('text-gray-300');
    boldSettings.Item.Class.push('hover:text-white');
    boldSettings.Item.Class.push('hover:bg-gray-100/10');
    
    let mounted = $derived(false);
    
    onMount(() => {
        mounted = true;
    });
</script>

<li>
    {#if mounted}
        <ContextMenu bind:this={myMenu} settings={boldSettings} >
            {#each actions as action}
                <Item class={action.isDestructive ? 'text-red-500 hover:bg-red-500/50 hover:text-white' : ''} >
                    <button onclick={() => {action.action(id, name)}}>
                        {action.label}
                    </button>
                </Item>
            {/each}
        </ContextMenu>
    {/if}
    
    <a href="{link}"
       oncontextmenu={(e) => {
            myMenu.show(e);
       }}
    >
        <div class="flex 
                    p-2.5
                    pl-5
                    pr-3
                    w-full
                    border-r-0
                    border-l-0
                    border-t-0
                    backdrop-brightness-125
                    hover:bg-blue-700 
                    active:bg-blue-900 
                    border-gray-400
                    {isTop    ? ' rounded-t-lg' : ''} 
                    {isBottom ? ' rounded-b-lg border-0' : 'border'}"
        >
            <p class="flex-1">{name}</p>
            <div class="flex">
                <p class="inset-e-10 text-gray-600">{subtitle}</p>
                <div class="flex flex-col">
                    <svg class="flex-1 w-4 stroke-gray-600"
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
        </div>
    </a>
</li>