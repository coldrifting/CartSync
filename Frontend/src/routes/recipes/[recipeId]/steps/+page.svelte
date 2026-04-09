<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAddStep from "$lib/components/modal/recipes/steps/ModalAddStep.svelte";
    import ModalEditStep from "$lib/components/modal/recipes/steps/ModalEditStep.svelte";

    let {data}: PageProps = $props();

    let headerActions: HeaderAction[] = [
        {
            label: "Add Step", icon: 'fa-plus', action: () => {
                modalAddStep.show();
            }
        }
    ];

    let items: SortableItem[] = $derived(data.recipe.steps.map(recipeStep => {
        return {
            id: recipeStep.id,
            name: recipeStep.content,
            subtitle: recipeStep.sortOrder.toString(),
            isContent: true,
            isImage: recipeStep.isImage,
            actionRight: {
                label: 'Edit',
                icon: 'fa-pencil',
                color: 'success',
                action: () => modalEditStep.show(recipeStep.id, recipeStep.content),
            }
        } as SortableItem;
    }))

    let reorderForm: HTMLFormElement;

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
    
    let modalAddStep: ModalAddStep
    let modalEditStep: ModalEditStep
</script>

<svelte:head>
    <title>Recipes - {data.recipe.name} - Steps</title>
</svelte:head>

<ModalAddStep bind:this={modalAddStep} action="addStep"/>
<ModalEditStep bind:this={modalEditStep} action="editStep"/>

<Header back={[`/recipes/${data.recipe.id}`, 'Recipe']} 
        title={data.recipe.name} 
        subtitle="Recipe Steps"
        headerActions={headerActions}/>

<ReorderableList listName="RecipeSteps" items={items} onReorder={onReorder}/>
<form method="POST"
      action="?/reorderStep"
      bind:this={reorderForm}
      use:enhance>
    <input hidden name="id" bind:value={reorderId}/>
    <input hidden name="stepSortOrder" bind:value={reorderIndex}/>
</form>