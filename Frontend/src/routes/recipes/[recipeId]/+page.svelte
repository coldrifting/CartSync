<script lang="ts">
    import Amount from "$lib/models/Amount.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import ModalAddRecipeEntry from "$lib/components/modal/recipes/ModalAddRecipeEntry.svelte";
    import ModalEditRecipeEntry from "$lib/components/modal/recipes/ModalEditRecipeEntry.svelte";
    import type Prep from "$lib/models/Prep.ts";
    import {get, patch} from "$lib/functions/requests.js";
    import HeadingButton from "$lib/components/HeadingButton.svelte";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import {page} from "$app/state";
    import type RecipeDetails from "$lib/models/RecipeDetails.ts";
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import {AllValidItems} from "$lib/models/ValidItemsAndPreps.js";
    import {SvelteMap} from "svelte/reactivity";
    import type {Snapshot} from "@sveltejs/kit";

    function getHost(url: string): string {
        return url.toLowerCase()
            .replace("http://", "")
            .replace("https://", "")
            .replace("www.", "")
            .split('/', 2)[0];
    }

    const client = useQueryClient()

    const queryRecipe = createQuery(() => ({
        queryKey: ['recipes', page.params.recipeId],
        queryFn: () => get<RecipeDetails>(`/api/recipes/${page.params.recipeId}`, fetch),
    }))

    const queryItems = createQuery(() => ({
        queryKey: ['items'],
        queryFn: () => get<ItemDetails[]>(`/api/items`, fetch),
    }))

    // TODO - Persist state when going to steps view
    let checked = new SvelteMap<string, boolean>();

    function getEntryMappings(recipeDetails: RecipeDetails, validItemsAndPreps: AllValidItems): Record<string, any> {
        let mapping: Record<string, any> = {};
        recipeDetails.sections.forEach(section => {
            section.entries.forEach(entry => {
                let preps: (Prep | null)[] = (validItemsAndPreps.sections.at(-1)?.items.find(item => item.id === entry.item.id)?.preps) ?? [];

                mapping[entry.id] = {
                    sectionId: section.id,
                    item: entry.item,
                    prep: entry.prep,
                    preps: preps,
                    amount: entry.amount
                }

            })
        })

        return mapping;
    }

    function showEditDialog(id: string, recipeDetails: RecipeDetails, validItemsAndPreps: AllValidItems, entryMappings: Record<string, any>) {
        let mapping = entryMappings[id];

        const allPreps = validItemsAndPreps.sections.at(-1)?.items.find(item => item.id === mapping.item.id)?.preps ?? [];
        const sectionUsedPreps = recipeDetails.sections.find(s => s.id == mapping.sectionId)?.entries.filter(e => e.item.id == mapping.item.id).map(i => i.prep) ?? [];
        const remainingPrepIds = allPreps.map(p => p?.id).filter(a => !sectionUsedPreps.map(p => p?.id).includes(a));

        const remainingPreps = allPreps.filter(s => remainingPrepIds.includes(s?.id));

        editDialog?.show(
            id,
            mapping.item.name,
            remainingPreps,
            mapping.prep?.id,
            mapping.amount.unitType,
            mapping.amount.fraction
        );
    }

    let headerActions: HeaderAction[] = [
        {
            label: "Add", icon: 'fa-plus', color: 'primary', action: () => {
                addDialog?.show();
            }
        }
    ];

    let addDialog: ModalAddRecipeEntry | undefined = $state(undefined)
    let editDialog: ModalEditRecipeEntry | undefined = $state(undefined)
    let renameDialog: ModalRename;

    let urlEditDialog: ModalRename;

    async function renameSectionAction(id: string, val: string) {
        await patch(`/api/recipes/sections/${id}/edit`, {"/Name": val});
        await client.invalidateQueries({queryKey: ['recipes', page.params.recipeId]});
    }

    async function urlEditAction(id: string, val: string) {
        await patch(`/api/recipes/${id}/edit`, {"/Url": val});
        await client.invalidateQueries({queryKey: ['recipes', page.params.recipeId]});
    }
</script>

<svelte:head>
    {#if queryRecipe.isSuccess}
        <title>Recipes - {queryRecipe.data.name}</title>
    {/if}
</svelte:head>

<ModalRename bind:this={renameDialog} type="Recipe Section" renameAction={renameSectionAction}/>
<ModalRename bind:this={urlEditDialog} type="Recipe URL" verb="Update" renameAction={urlEditAction}/>

{#if queryRecipe.isLoading || queryItems.isLoading}
    <Header back={['/recipes', 'Recipes']} title="Recipe" subtitle="Recipe Ingredients" headerActions={headerActions}/>
    <LoadingPage/>
{:else if queryRecipe.isError || queryItems.isError}
    <Header back={['/recipes', 'Recipes']} title="Recipe" subtitle="Recipe Ingredients" headerActions={headerActions}/>
    <p>ErrorRecipe: {queryRecipe.error?.message}</p>
    <p>ErrorItems: {queryItems.error?.message}</p>
{:else if queryRecipe.isSuccess && queryItems.isSuccess}
    <Header back={['/recipes', 'Recipes']} title={queryRecipe.data.name} subtitle="Recipe Ingredients" headerActions={headerActions}/>

    {@const validItemsAndPreps = AllValidItems.fromData(queryRecipe.data, queryItems.data)}
    {@const entryMappings = getEntryMappings(queryRecipe.data, validItemsAndPreps)}

    <ModalAddRecipeEntry bind:this={addDialog} recipeId={page.params.recipeId ?? ""}
                         sections={queryRecipe.data.sections} items={validItemsAndPreps}/>
    <ModalEditRecipeEntry bind:this={editDialog} recipeId={page.params.recipeId ?? ""}/>

    <h4>Details</h4>
    <ul>
        <ListItemLink label="Steps"
                      href="/recipes/{page.params.recipeId}/steps"
                      showArrow={true}/>

        <ListItemLink label="Url"
                      info={getHost(queryRecipe.data.url)}
                      href={queryRecipe.data.url}
                      isExternalLink={true}
                      actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => urlEditDialog.show(page.params.recipeId ?? "", queryRecipe.data.url)
                         }}/>
    </ul>

    {#if queryRecipe.data.sections.length === 1}
        <h4>Ingredients</h4>
        <ul>
            {#each queryRecipe.data.sections[0].entries as entry}
                <ListItemCheckbox id={entry.id}
                                  label={entry.item.name}
                                  info={Amount.asString(entry.amount)}
                                  subInfo={entry.prep?.name}
                                  name="RecipeEntry"
                                  checked={checked.get(entry.id) ?? false}
                                  actionRight={{
                                    label: 'Edit', 
                                    icon: 'fa-pencil', 
                                    color: 'success', 
                                    action: () => showEditDialog(entry.id, queryRecipe.data, validItemsAndPreps, entryMappings)
                                  }}
                                  onValueChange={(id, value) => {  
                                      checked.set(id, value); return new Promise(() => {}) 
                                  }}/>
            {/each}
        </ul>
    {:else}
        {#each queryRecipe.data.sections as section}
            <HeadingButton onclick={() => {renameDialog.show(section.id, section.name)}}>
                {section.name}
            </HeadingButton>
            <ul>
                {#each section.entries as entry}
                    <ListItemCheckbox id={entry.id}
                                      label={entry.item.name}
                                      info={Amount.asString(entry.amount)}
                                      subInfo={entry.prep?.name}
                                      name="RecipeEntry"
                                      checked={checked.get(entry.id) ?? false}
                                      actionRight={{
                                        label: 'Edit', 
                                        icon: 'fa-pencil', 
                                        color: 'success', 
                                        action: () => showEditDialog(entry.id, queryRecipe.data, validItemsAndPreps, entryMappings)
                                      }}
                                      onValueChange={(id, value) => {  
                                          checked.set(id, value); return new Promise(() => {}) 
                                      }}/>
                {/each}
            </ul>
        {/each}
    {/if}
{/if}