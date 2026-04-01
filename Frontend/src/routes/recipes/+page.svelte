<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/Header.svelte";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";
    let {data}: PageProps = $props();
    
    let recipeId: string = $state('');
    let recipeIsPinned: boolean = $state(false);
    let recipePinForm: HTMLFormElement;
    
    let togglePinAction = (id: string, _: string | undefined) => {
        recipeId = id;
        recipeIsPinned = !(data.allRecipes.filter(r => r.recipeId === id)[0].isPinned);
        tick().then(() => {
            recipePinForm.requestSubmit()
        });
    };
    
    const commonContextActions: ContextAction[] = [
        { label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, value: string | undefined) => {deleteDialog.show(id, value)} },
    ];
    
    const pinnedContextActions: ContextAction[] = [
		{ label: "Unpin", action: togglePinAction},
        ...commonContextActions
    ];
    
    const unpinnedContextActions: ContextAction[] = [
		{ label: "Pin", action: togglePinAction},
        ...commonContextActions
    ];
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    const headerActions: HeaderAction[] = [
        {label: "Add Recipe", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
</script>

<svelte:head>
    <title>Recipes</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addRecipe" header="Add Recipe" labelAdd="Recipe Name" />
<ModalRename bind:this={renameDialog} action="renameRecipe" header="Rename Recipe" labelRename="Recipe Name" />
<ModalDelete bind:this={deleteDialog} action="deleteRecipe" header="Delete Recipe" warning="The recipe [Name] will be deleted!" />

<Header title="Recipes" headerActions={headerActions} />

<form method="POST"
      action="?/toggleRecipePin"
      bind:this={recipePinForm}
      use:enhance>
    <input hidden name="id" bind:value={recipeId}/>
    <input hidden name="isPinned" type="checkbox" bind:checked={recipeIsPinned}/>
    <input hidden type="submit"/>
</form>

<h4>Pinned</h4>
<ul>
    {#each data.pinnedRecipes as recipe}
        <ListElementLink id={recipe.recipeId}
                         label={recipe.recipeName}
                          link="/recipes/{recipe.recipeId}"
                          contextActions={pinnedContextActions}
        />
    {/each}
</ul>

<h4>Unpinned</h4>
<ul>
    {#each data.unPinnedRecipes as recipe}
        <ListElementLink id={recipe.recipeId}
                         label={recipe.recipeName}
                          link="/recipes/{recipe.recipeId}"
                          contextActions={unpinnedContextActions}
        />
    {/each}
</ul>