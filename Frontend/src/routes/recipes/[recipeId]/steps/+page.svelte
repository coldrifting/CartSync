<script lang="ts">
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAddStep from "$lib/components/modal/recipes/steps/ModalAddStep.svelte";
    import ModalEditStep from "$lib/components/modal/recipes/steps/ModalEditStep.svelte";
    import {patch} from "$lib/functions/requests.js";

    let {data}: PageProps = $props();

    let headerActions: HeaderAction[] = [
        {
            label: "Add", icon: 'fa-plus', color: 'primary', action: () => {
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

    async function onReorder(stepId: string, newIndex: number) {
        await patch(`/api/recipes/steps/${stepId}/edit`, {'/SortOrder': newIndex});
    }
    
    let modalAddStep: ModalAddStep
    let modalEditStep: ModalEditStep
</script>

<svelte:head>
    <title>Recipes - {data.recipe.name} - Steps</title>
</svelte:head>

<ModalAddStep bind:this={modalAddStep} recipeId={data.recipe.id}/>
<ModalEditStep bind:this={modalEditStep}/>

<Header back={[`/recipes/${data.recipe.id}`, 'Recipe']} 
        title={data.recipe.name} 
        subtitle="Recipe Steps"
        headerActions={headerActions}/>

<ReorderableList listName="RecipeSteps" items={items} onReorder={onReorder}/>
