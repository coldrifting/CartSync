<script lang="ts">
    import Droppable from './Droppable.svelte';
    import DraggableItem from './DraggableItem.svelte'
    import {CollisionPriority} from '@dnd-kit/abstract';
    import {DragDropProvider, DragOverlay} from '@dnd-kit-svelte/svelte';
    import {move} from '@dnd-kit/helpers';
    import {RestrictToWindowEdges} from '@dnd-kit-svelte/svelte/modifiers';
    import {KeyboardSensor, PointerSensor} from '@dnd-kit-svelte/svelte';

    const sensors = [PointerSensor, KeyboardSensor];

    interface HasIndex {
        id: string;
        index: number;
    }

    interface Props {
        listName: string;
        items: SortableItem[];
        onReorder: (id: string, newIndex: number) => void;
    }

    let currentDragIndex: number = $state(-1);
    const onBeforeDragStart = (event: any) => {
        let source: HasIndex = event.operation.source;
        currentDragIndex = source.index;
    }
    const onDragOver = (event: any) => {
        items = move(items, event);
    }
    const onDragEnd = (event: any) => {
        setTimeout(() => {
            let source: HasIndex = event.operation.source;
            if (source.index != currentDragIndex) {
                onReorder(source.id, source.index)
            }
        }, 200);
    }

    let {listName, items, onReorder}: Props = $props();
</script>

<DragDropProvider
        {sensors}
        modifiers={[RestrictToWindowEdges]}
        onBeforeDragStart={onBeforeDragStart}
        onDragOver={onDragOver}
        onDragEnd={onDragEnd}>
    <Droppable id={listName}
               class="list"
               type="column"
               accept="item"
               collisionPriority={CollisionPriority.Low}>
        {#each items as item, index (item.id)}
            <DraggableItem {item}
                           numItems={items.length}
                           id={item.id}
                           index={() => index}
                           group={listName}
                           data={{group: listName}}
                           type="item"/>
        {/each}
    </Droppable>

    <DragOverlay>
        {#snippet children(source)}
            {@const item = items.find((item) => item.id === source.id)}
            {#if item !== undefined}
                <DraggableItem numItems={items.length} id={item.id} {item} index={0} isOverlay/>
            {/if}
        {/snippet}
    </DragOverlay>
</DragDropProvider>