<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    let {data}: PageProps = $props();
    
    let recipeId: string = $state('');
    let recipeIsPinned: boolean = $state(false);
    let recipePinForm: HTMLFormElement;
    
    let togglePinAction = (id: string) => {
        recipeId = id;
        recipeIsPinned = !(data.allRecipes.filter(recipe => recipe.id === id)[0].isPinned);
        tick().then(() => {
            recipePinForm.requestSubmit()
        });
    };
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const headerActions: HeaderAction[] = [
        {label: "Add Recipe", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
</script>

<svelte:head>
    <title>Recipes</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Recipe" />
<ModalRename bind:this={renameDialog} type="Recipe" warning="The recipe [Name] will be deleted!" />

<Header title="Recipes" headerActions={headerActions} />

<form method="POST"
      action="?/toggleRecipePin"
      bind:this={recipePinForm}
      use:enhance>
    <input hidden name="id" bind:value={recipeId}/>
    <input hidden name="isPinned" type="checkbox" bind:checked={recipeIsPinned}/>
    <input hidden type="submit"/>
</form>

{#if data.pinnedRecipes.length > 0}
    <h4>Pinned</h4>
    <ul>
        {#each data.pinnedRecipes as recipe}
            <ListItemLink label={recipe.name}
                             href="/recipes/{recipe.id}"
                             actionLeft={{
                                        label: 'Unpin', 
                                        icon: 'fa-star', 
                                        color: 'warning', 
                                        action: () => togglePinAction(recipe.id)
                                     }}
                             actionRight={{
                                        label: 'Edit', 
                                        icon: 'fa-pencil', 
                                        color: 'success', 
                                        action: () => renameDialog.show(recipe.id, recipe.name, true)
                                     }}/>
        {/each}
    </ul>
{/if}

{#if data.unPinnedRecipes.length > 0}
    <h4>Unpinned</h4>
    <ul>
        {#each data.unPinnedRecipes as recipe}
            <ListItemLink label={recipe.name}
                             href="/recipes/{recipe.id}"
                             actionLeft={{
                                        label: 'Pin', 
                                        icon: 'fa-star-o', 
                                        color: 'warning', 
                                        action: () => togglePinAction(recipe.id)
                                     }}
                             actionRight={{
                                        label: 'Edit', 
                                        icon: 'fa-pencil', 
                                        color: 'success', 
                                        action: () => renameDialog.show(recipe.id, recipe.name, true)
                                     }}/>
        {/each}
    </ul>
{/if}