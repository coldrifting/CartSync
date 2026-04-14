<script lang="ts">
    import type {PageProps} from './$types';
    import Amount from "$lib/models/Amount.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import ModalAddRecipeEntry from "$lib/components/modal/recipes/ModalAddRecipeEntry.svelte";
    import ModalEditRecipeEntry from "$lib/components/modal/recipes/ModalEditRecipeEntry.svelte";
    import type Prep from "$lib/models/Prep.ts";
    import {patch} from "$lib/functions/requests.js";
    import HeadingButton from "$lib/components/HeadingButton.svelte";

    function getHost(url: string): string {
        return url.toLowerCase()
            .replace("http://", "")
            .replace("https://", "")
            .replace("www.", "")
            .split('/', 2)[0];
    }

    let {data}: PageProps = $props();

    let urlHost: string = $derived(getHost(data.recipe.url));

    let entryMappings: Record<string, any> = $derived.by(() => {
        let mapping: Record<string, any> = {};
        data.recipe.sections.forEach(section => {
            section.entries.forEach(entry => {
                let preps: (Prep | null)[] = (data.validItemsAndPreps.sections.at(-1)?.items.find(item => item.id === entry.item.id)?.preps) ?? [];

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
    });

    const showEditDialog = (id: string) => {
        let mapping = entryMappings[id];

        const allPreps = data.validItemsAndPreps.sections.at(-1)?.items.find(item => item.id === mapping.item.id)?.preps ?? [];
        const sectionUsedPreps = data.recipe.sections.find(s => s.id == mapping.sectionId)?.entries.filter(e => e.item.id == mapping.item.id).map(i => i.prep) ?? [];
        const remainingPrepIds = allPreps.map(p => p?.id).filter(a => !sectionUsedPreps.map(p => p?.id).includes(a));

        const remainingPreps = allPreps.filter(s => remainingPrepIds.includes(s?.id));
        
        editDialog.show(
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
                addDialog.show();
            }
        }
    ];

    let checkedEntries: boolean[] = $derived.by(() => {
        data.recipe.sections;
        return [];
    });

    let addDialog: ModalAddRecipeEntry;
    let editDialog: ModalEditRecipeEntry;
    let renameDialog: ModalRename;

    let urlEditDialog: ModalRename;

    async function renameSectionAction(id: string, val: string) {
        await patch(`/api/recipes/sections/${id}/edit`, {"/Name": val});
    }

    async function urlEditAction(id: string, val: string) {
        await patch(`/api/recipes/${id}/edit`, {"/Url": val});
    }
</script>

<svelte:head>
    <title>Recipes - {data.recipe.name}</title>
</svelte:head>

<ModalAddRecipeEntry bind:this={addDialog} recipeId={data.recipe.id} sections={data.recipe.sections} items={data.validItemsAndPreps}/>
<ModalEditRecipeEntry bind:this={editDialog}/>

<ModalRename bind:this={renameDialog} type="Recipe Section" renameAction={renameSectionAction}/>
<ModalRename bind:this={urlEditDialog} type="Recipe URL" verb="Update" renameAction={urlEditAction}/>

<Header back={['/recipes', 'Recipes']} title={data.recipe.name} subtitle="Recipe Ingredients"
        headerActions={headerActions}/>

<h4>Details</h4>
<ul>
    <ListItemLink label="Steps"
                  href="/recipes/{data.recipe.id}/steps"
                  showArrow={true}/>

    <ListItemLink label="Url"
                  info={urlHost}
                  href={data.recipe.url}
                  isExternalLink={true}
                  actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => urlEditDialog.show(data.recipe.id, data.recipe.url)
                         }}/>
</ul>

{#if data.recipe.sections.length === 1}
    <h4>Ingredients</h4>
    <ul>
        {#each data.recipe.sections[0].entries as entry, i}
            <ListItemCheckbox id={entry.id}
                              label={entry.item.name}
                              info={Amount.asString(entry.amount)}
                              subInfo={entry.prep?.name}
                              name="RecipeEntry"
                              checked={checkedEntries[i]}
                              actionRight={{
                                    label: 'Edit', 
                                    icon: 'fa-pencil', 
                                    color: 'success', 
                                    action: () => showEditDialog(entry.id)
                                 }}/>
        {/each}
    </ul>
{:else}
    {#each data.recipe.sections as section, i}
        <HeadingButton onclick={() => {renameDialog.show(section.id, section.name)}}>
            {section.name}
        </HeadingButton>
        <ul>
            {#each section.entries as entry, j}
                <ListItemCheckbox id={entry.id}
                                  label={entry.item.name}
                                  info={Amount.asString(entry.amount)}
                                  subInfo={entry.prep?.name}
                                  name="RecipeEntry"
                                  checked={checkedEntries[(j * i) + j]}
                                  actionRight={{
                                        label: 'Edit', 
                                        icon: 'fa-pencil', 
                                        color: 'success', 
                                        action: () => showEditDialog(entry.id)
                                     }}/>
            {/each}
        </ul>
    {/each}
{/if}