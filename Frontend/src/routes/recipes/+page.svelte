<script lang="ts">
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import {del, patch, post} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";
    let {data}: PageProps = $props();
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const headerActions: HeaderAction[] = [
        {label: "Add", icon: "fa-plus", color: 'primary', action: () => {addDialog.show()}}
    ];
    
    async function addAction(value: string) {
        await post('/api/recipes/add', {name: value});
    }
    
    async function renameAction(id: string, value: string) {
        await patch(`/api/recipes/${id}/edit`, {"/Name": value});
    }
    
    async function deleteAction(id: string) {
        await del(`/api/recipes/${id}/delete`);
    }
    
    async function pinAction(id: string, value: boolean) {
        await patch(`/api/recipes/${id}/edit`, {"/IsPinned": value});
        await invalidateAll();
    }
</script>

<svelte:head>
    <title>CartSync - Recipes</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Recipe" addAction={addAction} />
<ModalRename bind:this={renameDialog} type="Recipe" renameAction={renameAction} deleteAction={deleteAction} warning="The recipe [Name] will be deleted!" />

<Header title="Recipes" headerActions={headerActions} />

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
                                        action: () => pinAction(recipe.id, !recipe.isPinned)
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
                                        action: () => pinAction(recipe.id, !recipe.isPinned)
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