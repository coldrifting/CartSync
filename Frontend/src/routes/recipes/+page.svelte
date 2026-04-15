<script lang="ts">
    import Header from "$lib/components/nav/Header.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import {del, get, mutate, patch, post} from "$lib/functions/requests.js";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import type Recipe from "$lib/models/Recipe.ts";
    import LoadingPage from "$lib/components/LoadingPage.svelte";

    const client = useQueryClient()

    const queryRecipes = createQuery(() => ({
        queryKey: ['recipes'],
        queryFn: () => get<Recipe[]>('/api/recipes', fetch),
    }))

    let addDialog: ModalAdd
    let renameDialog: ModalRename

    const headerActions: HeaderAction[] = [
        {
            label: "Add", icon: "fa-plus", color: 'primary', action: () => {
                addDialog.show()
            }
        }
    ];

    async function addAction(value: string) {
        await post('/api/recipes/add', {name: value});
        await client.invalidateQueries({queryKey: ['recipes']});
    }

    async function renameAction(id: string, value: string) {
        await patch(`/api/recipes/${id}/edit`, {"/Name": value});
        await client.invalidateQueries({queryKey: ['recipes']});
    }

    async function deleteAction(id: string) {
        await del(`/api/recipes/${id}/delete`);
        await client.invalidateQueries({queryKey: ['recipes']});
    }

    const togglePinRecipeMutate = mutate<[string, boolean], Recipe[]>(
        client, 
        ['recipes'],
        ([id, value]) => patch(`/api/recipes/${id}/edit`, {"/IsPinned": value}),
        (query, [id, value]) => {
            const clone = structuredClone(query);
            const index = clone.map(a => a.id).indexOf(id);
            if (index !== -1) {
                clone[index].isPinned = value;
            }
            
            return clone;
        }
    );
    
    async function pinAction(id: string, value: boolean) {
        togglePinRecipeMutate.mutate([id, value])
    }
</script>

<svelte:head>
    <title>CartSync - Recipes</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Recipe" addAction={addAction}/>
<ModalRename bind:this={renameDialog} type="Recipe" renameAction={renameAction} deleteAction={deleteAction}
             warning="The recipe [Name] will be deleted!"/>

<Header title="Recipes" headerActions={headerActions}/>

{#if queryRecipes.isLoading}
    <LoadingPage/>
{:else if queryRecipes.isError}
    <p>Error: {queryRecipes.error?.message}</p>
{:else if queryRecipes.isSuccess}
    {@const pinnedRecipes = queryRecipes.data.filter(r => r.isPinned)}
    {@const unPinnedRecipes = queryRecipes.data.filter(r => !r.isPinned)}
    {#if pinnedRecipes.length > 0}
        <h4>Pinned</h4>
        <ul>
            {#each pinnedRecipes as recipe}
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

    {#if unPinnedRecipes.length > 0}
        <h4>Unpinned</h4>
        <ul>
            {#each unPinnedRecipes as recipe}
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
{/if}