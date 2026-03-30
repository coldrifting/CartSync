<script lang="ts">
	import {useSortable, type UseSortableInput} from '@dnd-kit-svelte/svelte/sortable';
	import ContextMenu, {Item} from "svelte-contextmenu";
    import {onMount, untrack} from "svelte";

	interface Props extends UseSortableInput {
		item: SortableItem;
		isOverlay?: boolean;
	}

	let {item, isOverlay = false, ...rest}: Props = $props();

	let restx = $state(untrack(() => rest))
    
	const {ref, isDragging} = useSortable({...restx, feedback: 'move'});
	
    let myMenu: ContextMenu | null = $state(null)
    
    let mounted = $derived(false);
    
    onMount(() => {
        mounted = true;
    });
    
    const onContextMenu = (e: MouseEvent) => {
       if (!e.shiftKey) {
            myMenu?.show(e);
       }
    }
</script>


{#if mounted}
    <ContextMenu bind:this={myMenu}>
        {#each item.contextActions as action}
            <Item on:click={() => {action.action(item.id, item.name)}} class={action.isDestructive ? 'delete' : ''}>
                {action.label}
            </Item>
        {/each}
    </ContextMenu>
{/if}

<div class="relative select-none draggable-list-item" 
     oncontextmenu={onContextMenu}
     role="button"
     tabindex={0}
     {@attach ref} >
	<!-- Original element - becomes invisible during drag but maintains dimensions -->
	<div class={['d-flex', {invisible: isDragging.current && !isOverlay}]}>
        <span>{item.name}</span>
        <span class="ms-auto text-secondary">{item.subtitle}</span>
	</div>
</div>