<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Amount from "$lib/scripts/classes/Amount.js";
    import Header from "$lib/components/Header.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";
    import ListElementCheckbox from "$lib/components/ListElementCheckbox.svelte";
    import ModalAddIngredient from "$lib/components/modal/ingredients/ModalAddIngredient.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalEditIngredient from "$lib/components/modal/ingredients/ModalEditIngredient.svelte";
    import type Prep from "$lib/scripts/classes/Prep.ts";

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
                let preps: (Prep | null)[] = (data.validItemsAndPreps.sections.at(-1)?.validItems.find(i => i.itemId === entry.item.itemId)?.preps) ?? [];
                
                mapping[entry.recipeSectionEntryId] = {
                    sectionId: section.recipeSectionId,
                    item: entry.item,
                    prep: entry.prep,
                    preps: preps,
                    amount: entry.amount
                }
                
            })
        })
        
        return mapping;
    });
    
    let contextActions: ContextAction[] = [
        {
            label: "Edit", action: (id: string, _: string | undefined) => {
                let mapping = entryMappings[id];
                
                editDialog.show(
                    id, 
                    mapping.sectionId, 
                    mapping.item.itemName,
                    mapping.preps,
                    mapping.prep?.prepId,
                    mapping.amount.unitType,
                    mapping.amount.fraction
                );
            }
        },
        {
            label: "Delete", action: (id: string, _: string | undefined) => {
                let mapping = entryMappings[id];
                
                deleteEntryId = id;
                
                // TODO - Change API route conventions to avoid this extra variable
                deleteSectionId = mapping.sectionId;
                tick().then(() => deleteForm.requestSubmit());
            }
        }
    ];

    let urlContextActions: ContextAction[] = [
        { label: "Edit", action: (id: string, _: string | undefined) => {
            urlEditDialog.show(id, data.recipe.url);
        }}
    ]
    
    let headerActions: HeaderAction[] = [
        {
            label: "Add Ingredient", icon: 'fa-plus', action: () => {
                addDialog.show();
            }
        }
    ];

    let checkedEntries = $state([]);

    let addDialog: ModalAddIngredient;
    let editDialog: ModalEditIngredient;
    let renameDialog: ModalRename;
    
    let urlEditDialog: ModalRename;
    
    let deleteEntryId = $state('');
    let deleteSectionId = $state('');
    let deleteForm: HTMLFormElement;
</script>

<svelte:head>
    <title>Recipes - {data.recipe.recipeName}</title>
</svelte:head>

<ModalAddIngredient bind:this={addDialog} action="addRecipeEntry" header="Add Recipe Entry"
                    sections={data.recipe.sections} items={data.validItemsAndPreps}/>

<ModalEditIngredient bind:this={editDialog} action="editRecipeEntry" header="Edit Recipe Entry"/>

<ModalRename bind:this={renameDialog} action="renameRecipeSection" labelRename="New Section Name"
             header="Rename Recipe Section"/>

<ModalRename bind:this={urlEditDialog} action="editRecipeUrl" labelRename="Recipe URL"
             header="Edit Recipe URL"/>

<Header back={['/recipes', 'Recipes']} title={data.recipe.recipeName} subtitle="Recipe Ingredients"
        headerActions={headerActions}/>

<h4>Details</h4>
<ul>
    <ListElementLink id="Steps" label="Steps" link="/recipes/{data.recipe.recipeId}/steps"/>
    <ListElementLink id="Url" label="Url" info={urlHost} link={data.recipe.url} isExternalLink={true} contextActions={urlContextActions} />
</ul>

{#if data.recipe.sections.length === 1}
    <h4>Ingredients</h4>
    <ul>
        {#each data.recipe.sections[0].entries as entry, i}
            <ListElementCheckbox id={entry.recipeSectionEntryId}
                                 label={entry.item.itemName}
                                 info={Amount.asString(entry.amount)}
                                 subInfo={entry.prep?.prepName}
                                 name="RecipeEntry"
                                 checked={checkedEntries[i]}
                                 contextActions={contextActions}/>
        {/each}
    </ul>
{:else}
    {#each data.recipe.sections as section, i}
        <button class="heading-button"
                onclick={() => {renameDialog.show(section.recipeSectionId, section.recipeSectionName)}}>
            {section.recipeSectionName}
        </button>
        <ul>
            {#each section.entries as entry, j}
                <ListElementCheckbox id={entry.recipeSectionEntryId}
                                     label={entry.item.itemName}
                                     info={Amount.asString(entry.amount)}
                                     subInfo={entry.prep?.prepName}
                                     name="RecipeEntry"
                                     checked={checkedEntries[(j * i) + j]}
                                     contextActions={contextActions}/>
            {/each}
        </ul>
    {/each}
{/if}

<form method="POST"
      action="?/deleteRecipeEntry"
      bind:this={deleteForm}
      use:enhance>
    <input hidden name="entryId" bind:value={deleteEntryId}/>
    <input hidden name="sectionId" bind:value={deleteSectionId}/>
</form>