<script lang="ts">
    import Header from "$lib/components/nav/Header.svelte";
    import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAddStep from "$lib/components/modal/recipes/steps/ModalAddStep.svelte";
    import ModalEditStep from "$lib/components/modal/recipes/steps/ModalEditStep.svelte";
    import {get, mutate, patch} from "$lib/functions/requests.js";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import {page} from "$app/state";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import type RecipeDetails from "$lib/models/RecipeDetails.ts";

    const client = useQueryClient()

    const queryRecipe = createQuery(() => ({
        queryKey: ['recipes', page.params.recipeId],
        queryFn: () => get<RecipeDetails>(`/api/recipes/${page.params.recipeId}`, fetch),
    }))

    let headerActions: HeaderAction[] = [
        {
            label: "Add", icon: 'fa-plus', color: 'primary', action: () => {
                modalAddStep.show();
            }
        }
    ];

    function getStepsSortData(recipe: RecipeDetails) {
        return recipe.steps.map(recipeStep => {
            return {
                id: recipeStep.id,
                name: recipeStep.content,
                subtitle: (recipeStep.sortOrder + 1).toString(),
                isContent: true,
                isImage: recipeStep.isImage,
                actionRight: {
                    label: "Edit",
                    icon: "fa-pencil",
                    color: "success",
                    action: () => modalEditStep.show(recipeStep.id, recipeStep.content)
                }
            } as SortableItem;
        });
    }
    
    const stepOrderMutate = mutate<[string, number], RecipeDetails>(
        client, 
        ['recipes', page.params.recipeId],
        ([id, sortOrder]) => patch(`/api/recipes/steps/${id}/edit`, {'/SortOrder': sortOrder}),
        (query, [id, sortOrder]) => {
            const clone = structuredClone(query);
            
            const currentIndex = clone.steps.map(a => a.id).indexOf(id);
            const newIndex = sortOrder;
            
            let item = clone.steps.splice(currentIndex, 1)[0];
            clone.steps.splice(newIndex, 0, item);
            
            for (let i = 0; i < clone.steps.length; i++) {
                clone.steps[i].sortOrder = i;
            }
            
            return clone;
        }
    );

    async function onReorder(stepId: string, newIndex: number) {
        stepOrderMutate.mutate([stepId, newIndex]);
    }
    
    let modalAddStep: ModalAddStep
    let modalEditStep: ModalEditStep
</script>

<ModalAddStep bind:this={modalAddStep} recipeId={page.params.recipeId ?? ""}/>
<ModalEditStep bind:this={modalEditStep} recipeId={page.params.recipeId ?? ""}/>

<svelte:head>
    {#if queryRecipe.isSuccess}
        <title>Recipes - {queryRecipe.data.name} - Steps</title>
    {/if}
</svelte:head>

{#if queryRecipe.isLoading}
    <Header back={[`/recipes/${page.params.recipeId}`, 'Recipe']} 
            title="Recipe"
            subtitle="Recipe Steps"
            headerActions={headerActions}/>
    <LoadingPage/>
{:else if queryRecipe.isError}
    <Header back={[`/recipes/${page.params.recipeId}`, 'Recipe']} 
            title="Recipe"
            subtitle="Recipe Steps"
            headerActions={headerActions}/>
    <p>Error: {queryRecipe.error?.message}</p>
{:else if queryRecipe.isSuccess}
    <Header back={[`/recipes/${page.params.recipeId}`, 'Recipe']} 
            title={queryRecipe.data.name} 
            subtitle="Recipe Steps"
            headerActions={headerActions}/>
    
    <ReorderableList listName="RecipeSteps" items={getStepsSortData(queryRecipe.data)} onReorder={onReorder}/>
{/if}