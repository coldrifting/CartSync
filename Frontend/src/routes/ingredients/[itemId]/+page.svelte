<script lang="ts">
    import type {PageProps} from './$types';
    import LinkHeader from "$lib/components/LinkHeader.svelte";

    let {data}: PageProps = $props();
    let ingredient = $derived(data.ingredient);
</script>

<!-- TODO -->
<LinkHeader url="/ingredients" title="Ingredients"/>
<h2 class="text-center">{ingredient.itemName}</h2>
<p>{ingredient.itemTemp}</p>
<p>{ingredient.defaultUnitType}</p>

{#if ingredient.locations.length > 0}
    <span>Locations:</span>
    {#each ingredient.locations as location}
        <span>{location.aisleName} - {location.bay}</span>
        <br>
    {/each}
{:else}
    <span>Locations: (None)</span>
{/if}

<br>

{#if ingredient.preps.length > 0}
    <span>Preps:</span>
    {#each ingredient.preps as prep}
        <span>{prep.prepName}</span>
        <br>
    {/each}
{:else}
    <span>Preps: (None)</span>
{/if}