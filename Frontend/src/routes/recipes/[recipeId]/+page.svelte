<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Amount from "$lib/scripts/classes/Amount.js";
    import Header from "$lib/components/Header.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";
    import ListElementCheckbox from "$lib/components/ListElementCheckbox.svelte";

    let {data}: PageProps = $props();
    
    let contextActions: ContextAction[] = [
        {
            label: "Edit", action: (a: string, b: string | undefined) => {
                // TODO
                console.log(a + b)
            }
        },
        {
            label: "Delete", action: (a: string, b: string | undefined) => {
                // TODO
                console.log(a + b)
            }
        }
    ];
    
    let headerActions: HeaderAction[] = [
        {label: "Add Ingredient", icon: 'fa-plus', action: () => {
            // TODO
        }}
    ];
    
    let checkedEntries = $state([]);
</script>

<svelte:head>
    <title>Recipes - {data.recipe.recipeName}</title>
</svelte:head>

<Header back={['/recipes', 'Recipes']} title={data.recipe.recipeName} subtitle="Recipe Ingredients" headerActions={headerActions}/>

<h4>Details</h4>
<ListElementLink id="Steps" label="Steps" link="/recipes/{data.recipe.recipeId}/steps"/>

{#if data.recipe.sections.length === 1}
    <h4>Ingredients</h4>
    <ul>
        {#each data.recipe.sections[0].entries as entry, i}
            <ListElementCheckbox id={entry.item.itemId}
                                 label={entry.item.itemName}
                                 info={Amount.asString(entry.amount)}
                                 subInfo={entry.prep?.prepName}
                                 name="RecipeEntry"
                                 checked={checkedEntries[i]}
                                 contextActions={contextActions}/>
        {/each}
    </ul>
{/if}