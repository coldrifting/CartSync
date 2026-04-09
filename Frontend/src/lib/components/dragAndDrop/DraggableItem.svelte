<script lang="ts">
	import {useSortable, type UseSortableInput} from '@dnd-kit-svelte/svelte/sortable';
	import {untrack} from "svelte";
	import {Button} from "@sveltestrap/sveltestrap";

	interface Props extends UseSortableInput {
		numItems: number;
		item: SortableItem;
		isOverlay?: boolean;
	}

	let {numItems, item, isOverlay = false, ...rest}: Props = $props();

	let restx = $state(untrack(() => rest))
    
	const {ref, isDragging} = useSortable({...restx, feedback: 'move'});
	
	const clamp = (num: number, min: number, max: number) => Math.min(Math.max(num, min), max)
	
	const width: number = $derived.by(() => {
		return clamp(Number.parseFloat((numItems / 15).toFixed(2)), 1, 4);
	});
	
</script>

<div class="relative select-none l-item d-flex flex-row align-items-center {item.isContent ? 'expanded' : ''}" 

     {@attach ref} >
	<div class={['d-flex flex-row align-items-center', {invisible: isDragging.current && !isOverlay}]}>
        <span class='prefix me-3 ms-3 text-secondary' style="width: {width}rem">
			{item.subtitle}
		</span>
		{#if item.isImage}
			<img src={item.name} alt="recipe step"/>
		{:else}
        	<span class="me-auto">{item.name}</span>
		{/if}
	</div>
		{#if item.actionRight !== undefined}
			<Button aria-label={item.actionRight.label} 
					color={item.actionRight.color} 
                	class="rounded-5 m-auto me-2"
					type="button" 
					onclick={() => item.actionRight?.action()}>
				<i class="fa fa-lg {item.actionRight.icon}"></i>
			</Button>
		{/if}
</div>