<script lang="ts">
	import { enhance } from '$app/forms';
    import type {PageProps} from './$types';

    let {data}: PageProps = $props();
    let filterTerm = $state('');

    let filter = (items: IngredientByStore[]) => {
        if (!filterTerm) return items;
        let searchText = filterTerm.toLowerCase().trim();
        return items.filter(i => i.itemName.toLowerCase().includes(searchText));
    }
    
    let filteredIngredients: IngredientByStore[] = $derived(filter(data.ingredients));
</script>

<h1 class="text-4xl font-semibold">Ingredients</h1>
<form method="POST"
      action="?/addIngredient" 
      use:enhance={() => {
          console.log(filterTerm)
      }}>
    <label for="search" class="block mb-2.5 text-sm font-medium text-heading sr-only">Search</label>
    <div class="relative">
        <div class="absolute inset-y-0 flex items-center ps-3 pointer-events-none">
            <svg class="w-5 h-5 text-body" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24"
                 height="24" fill="none" viewBox="0 0 24 24">
                <path stroke="currentColor" stroke-linecap="round" stroke-width="2"
                      d="m21 21-3.5-3.5M17 10a7 7 0 1 1-14 0 7 7 0 0 1 14 0Z"/>
            </svg>
        </div>
        <input
                type="search"
                id="search"
                bind:value={filterTerm}
                class="text-lg mt-3 mb-3 rounded-lg block w-full p-2 ps-9 bg-neutral-secondary-medium border border-default-medium text-heading rounded-base focus:ring-brand focus:border-brand shadow-xs placeholder:text-body"
                placeholder="Search" 
                required/>
    </div>

<div>
    {#each filteredIngredients as ingredient, i}
        <a href="/ingredients/{ingredient.itemId}">
            <div class="
        flex 
        p-2.5
        pl-5
        pr-3
        border-r-0
        border-l-0
        border-t-0
        border-gray-400
        backdrop-brightness-125
        hover:bg-blue-700 
        {i === 0 ? ' rounded-t-lg' : ''} 
        {i === filteredIngredients.length - 1 ? ' rounded-b-lg border-0' : 'border'}">
                <p class="flex-1">{ingredient.itemName}</p>
                <div class="flex">
                    <p class="inset-e-10 text-gray-600">{ingredient.location?.aisleName ?? "(No Location)"}</p>
                    <div class="flex flex-col">
                        <svg class="flex-1 w-4 stroke-gray-600"
                             aria-hidden="true"
                             xmlns="http://www.w3.org/2000/svg"
                             width="16"
                             height="16"
                             fill="none"
                             viewBox="0 0 16 16">
                            <path stroke="strokeColor" stroke-linecap="round" stroke-width="1.5"
                                  d="M8,0 16,8 8,16"/>
                        </svg>
                    </div>
                </div>
            </div>
        </a>
    {/each}
    {#if filteredIngredients.length === 0}
            <input name="itemName" 
                   value="{filterTerm}" 
                   hidden />
            <button type="submit" 
                    aria-label="Add Ingredient"
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
                        w-full"
                    disabled="{filterTerm.trim().length === 0}">
                <span class="font-semibold">Add Ingredient</span>
            </button>
    {/if}
</div>
</form>

<style lang="postcss">
    @reference "tailwindcss";
    :global(html) {
        background-color: theme(--color-gray-100);
    }
</style>