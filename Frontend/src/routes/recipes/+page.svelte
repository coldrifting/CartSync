<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/Header.svelte";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import ListItem from "$lib/components/ListItem.svelte";
    import {tick} from "svelte";
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
    
    let pinnedContextActions: ContextAction[] = [
		{ label: "Unpin", action: togglePinAction},
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, value: string | undefined) => {deleteDialog.show(id, value)} },
    ];
    
    let unpinnedContextActions: ContextAction[] = [
		{ label: "Pin", action: togglePinAction},
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, value: string | undefined) => {deleteDialog.show(id, value)} },
    ];
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
</script>

<svelte:head>
    <title>Recipes</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addRecipe" header="Add Recipe" labelAdd="Recipe Name" />
<ModalRename bind:this={renameDialog} action="renameRecipe" header="Rename Recipe" labelRename="Recipe Name" />
<ModalDelete bind:this={deleteDialog} action="deleteRecipe" header="Delete Recipe" warning="The recipe [Name] will be deleted!" />

<Header title="Recipes" actions={[{label: "Add Recipe", icon: "fa-plus", action: () => {addDialog.show()}}]} />

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
        <ListItem name={recipe.recipeName}
                  id={recipe.recipeId}
                  link="/recipes/{recipe.recipeId}"
                  contextActions={pinnedContextActions}
        />
    {/each}
</ul>

<h4>Unpinned</h4>
<ul>
    {#each data.unPinnedRecipes as recipe}
        <ListItem name={recipe.recipeName}
                  id={recipe.recipeId}
                  link="/recipes/{recipe.recipeId}"
                  contextActions={unpinnedContextActions}
        />
    {/each}
</ul>