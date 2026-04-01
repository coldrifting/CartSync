<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/Header.svelte";
    import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import {tick} from "svelte";

    let {data}: PageProps = $props();

    let headerActions: HeaderAction[] = [
        {
            label: "Add Step", icon: 'fa-plus', action: () => {
                // TODO
            }
        }
    ];

    let contextActions: ContextAction[] = [
        {
            label: "Edit", action: (id, value) => { 
				// TODO
            }
        },
        {
            label: "Delete", action: (id, _) => {
                deleteId = id;
                tick().then(() => deleteForm.requestSubmit());
            }
        }
    ];

    let items: SortableItem[] = $derived(data.recipe.instructions.map(i => {
        return {
            id: i.recipeInstructionId,
            name: i.recipeInstructionContent,
            subtitle: i.sortOrder.toString(),
            isContent: true,
            isImage: i.isImage
        } as SortableItem;
    }))

    let deleteForm: HTMLFormElement;
    let reorderForm: HTMLFormElement;

    let deleteId = $state('');

    let reorderState = $state<{ id: string, index: number }>({id: "", index: -1})
    let reorderId = $derived(reorderState.id);
    let reorderIndex = $derived(reorderState.index);

    let onReorder = (id: string, newIndex: number) => {
        reorderState = {id: id, index: newIndex};
    }

    $effect(() => {
        if (reorderState.id != "" && reorderState.index != -1) {
            reorderForm.requestSubmit();
        }
    })
</script>

<svelte:head>
    <title>Recipes - {data.recipe.recipeName} - Instructions</title>
</svelte:head>

<Header back={[`/recipes/${data.recipe.recipeId}`, 'Recipe']} title={data.recipe.recipeName} subtitle="Recipe Steps"
        headerActions={headerActions}/>

<ReorderableList listName="RecipeInstructions" items={items} onReorder={onReorder} contextActions={contextActions}/>
<form method="POST"
      action="?/reorderInstruction"
      bind:this={reorderForm}
      use:enhance>
    <input hidden name="id" bind:value={reorderId}/>
    <input hidden name="instructionSortOrder" bind:value={reorderIndex}/>
</form>

<form method="POST"
      action="?/deleteInstruction"
      bind:this={deleteForm}
      use:enhance>
    <input hidden name="id" bind:value={deleteId}/>
</form>