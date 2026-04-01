<script lang="ts">
	import {useSortable, type UseSortableInput} from '@dnd-kit-svelte/svelte/sortable';
    import {onMount, untrack} from "svelte";

	interface Props extends UseSortableInput {
		item: SortableItem;
		isOverlay?: boolean;
	}

	let {item, isOverlay = false, ...rest}: Props = $props();

	let restx = $state(untrack(() => rest))
    
	const {ref, isDragging} = useSortable({...restx, feedback: 'move'});
	
</script>

<div class="relative select-none list-item {item.isContent ? 'expanded' : ''}" 
     role="button"
     tabindex={0}
     {@attach ref} >
	<!-- Original element - becomes invisible during drag but maintains dimensions -->
	<div class={['d-flex', {invisible: isDragging.current && !isOverlay}]}>
		{#if item.isImage}
			<img src={item.name} alt="recipe step"/>
		{:else}
        	<span>{item.name}</span>
		{/if}
        <span class='ms-auto subtitle'>{item.subtitle}</span>
	</div>
</div>