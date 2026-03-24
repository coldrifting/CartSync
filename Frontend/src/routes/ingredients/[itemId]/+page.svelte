<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
    let ingredient = $derived(data.ingredient);
</script>

<!-- TODO -->
<h1 class="text-4xl font-semibold">{ingredient.itemName}</h1>
<p>{ingredient.itemTemp}</p>
<p>{ingredient.defaultUnitType}</p>

{#each ingredient.locations as location}
    <p>{location.aisleName} - {location.bay}</p>
{/each}

{#each ingredient.preps as prep}
    <p>{prep.prepName}</p>
{/each}

<a 
        href="/ingredients"                        
        class="
            flex 
            p-2.5
            pl-5
            pr-3
            border-r-0
            border-l-0
            border-t-0
            bg-blue-500
            hover:bg-blue-700 
            active:bg-blue-900 
            backdrop-brightness-125
            rounded-lg
            w-52"

>&lt- Back</a>