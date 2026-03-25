<script lang="ts">
    import type {PageProps} from './$types';

    let {data}: PageProps = $props();
    let ingredient = $derived(data.ingredient);
</script>

<!-- TODO -->
<a href="/ingredients" class="text-decoration-none h2 title-navigation ">
    <div class="d-flex flex-row">
        <svg class="mt-2 text-primary"
             aria-hidden="true"
             xmlns="http://www.w3.org/2000/svg"
             width="24"
             height="24"
             fill="none"
             viewBox="0 0 16 16">
            <path stroke="strokeColor" stroke-linecap="round" stroke-width="2"
                  d="M8,16 2,8 8,0"/>
        </svg>
        <span>{ingredient.itemName}</span>
    </div>
</a>
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