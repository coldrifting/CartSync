<script lang="ts">
	import { enhance } from '$app/forms';
    import type {PageProps} from './$types';
    import ListItem from '$lib/ListItem.svelte';
    import ListButton from "$lib/ListButton.svelte";
    
    let {data}: PageProps = $props();
    
    let dialog: HTMLDialogElement;
    function onDialogClick(e: Event) {
        if (e.target === dialog) {
            dialog.close();
        }
    }
    
    let dialog2: HTMLDialogElement;
    function dialog2Close() {
        deleteId = '';
        itemInUseText = '';
        dialog2.close();
    }
    
    function onDialog2Click(e: Event) {
        if (e.target === dialog2) {
            dialog2Close();
        }
    }
    
    let filterTerm = $state('');

    let filter = (items: IngredientByStore[]) => {
        if (!filterTerm) return items;
        let searchText = filterTerm.toLowerCase().trim();
        return items.filter(i => i.itemName.toLowerCase().includes(searchText));
    }
    
    let filteredIngredients: IngredientByStore[] = $derived(filter(data.ingredients));
    
    let deleteId = $state('');
    let renameId = $state('');
    let newName = $state('');
    let itemInUseText = $state('');
    
    $effect(() => {
        if (deleteId.length != 0) {
            let tryDeleteForm = document.getElementById("tryDeleteForm") as HTMLFormElement;
            if (tryDeleteForm) {
                tryDeleteForm.requestSubmit();
            }
        }
    });
    
    let actions: ContextAction[] = [
        {
            label: "Rename", 
            isDestructive: false,
            action: (val, name) => {
                renameId = val;
                newName = name ?? "";
                dialog.showModal();
            }
        },
        {
            label: "Delete", 
            isDestructive: true,
            action: (val, _) => {
                deleteId = val;
            }
        }
    ];
</script>

<svelte:head>
	<title>Ingredients</title>
</svelte:head>

<dialog class="backdrop:backdrop-blur-xs absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-lg p-10 pb-5 mb-10 mt-2 bg-gray-700 rounded-2xl"
        bind:this={dialog} 
        onclick={onDialogClick}>
        <form 
                class="w-full" 
                method="POST" 
                action="?/rename" 
                id="renameForm" 
                use:enhance={() => {dialog.close()}}>
            <input hidden name="id" bind:value={renameId} />
            
            <h1 class="text-2xl mb-5">Rename Item</h1>
            
            <div class="mb-3 flex flex-row h-9">
                <label class="w-30" for="newName">Item Name</label>
                <input class="flex-1 p-3 rounded-lg bg-gray-900" 
                       name="newName"
                       bind:value={newName}
                        required />
            </div>
            <div class="mb-3 flex flex-row">
                <input type="submit" class="flex-1 h-10 rounded-lg bg-blue-500 hover:bg-blue-600 active:bg-blue-700" />
            </div>
        </form>
</dialog>


<dialog class="backdrop:backdrop-blur-xs absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 text-lg p-10 pb-5 mb-10 mt-2 bg-gray-700 rounded-2xl"
        bind:this={dialog2} 
        onclick={onDialog2Click}>
        <form 
                class="w-full" 
                method="POST" 
                action="?/delete" 
                id="deleteForm" 
                use:enhance={() => {dialog2Close()}}>
            <input hidden name="id" bind:value={deleteId} />
            
            <h1 class="text-2xl mb-5">Confirm Item Deletion</h1>
            
            <div class="mb-3 flex flex-row">
                <p class="whitespace-pre-line">{itemInUseText}</p>
            </div>
            <div class="mb-3 flex flex-row">
                <button onclick={dialog2Close} type="button" class="flex-1 h-10 rounded-lg bg-blue-500 hover:bg-blue-600 active:bg-blue-700" >Cancel</button>
                <button type="submit" class="flex-1 h-10 rounded-lg bg-red-500 hover:bg-red-600 active:bg-red-700" >Delete Item</button>
            </div>
        </form>
</dialog>

<form method="POST" 
      action="?/tryDelete" 
      id="tryDeleteForm" 	
        use:enhance={() => {
            return async ({ result, update }) => {
                if (result.type === 'failure' && result.data) {
                    itemInUseText = (result.data ?? "").toString()
                    dialog2.showModal();
                } else {
                    await update();
                }
            };
        }}
      >
    <input hidden name="id" bind:value={deleteId} />
    <input hidden type="submit" />
</form>

<h1 class="text-4xl font-semibold">Ingredients</h1>
<form method="POST"
      action="?/addIngredient" 
      use:enhance
>
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
        <ul>
            {#each filteredIngredients as ingredient, i}
                <input name="itemName" value="{filterTerm}" hidden />
                <ListItem name={ingredient.itemName}
                          id={ingredient.itemId}
                          subtitle={ingredient.location?.aisleName ?? "(No Location)"}
                          link="/ingredients/{ingredient.itemId}" 
                          isTop={i === 0}
                          isBottom={i === filteredIngredients.length - 1}
                          actions={actions}
                />
            {/each}
            {#if filteredIngredients.length === 0}
                <input name="itemName" value="{filterTerm}" hidden />
                <ListButton name="Add Ingredient"/>
            {/if}
        </ul>
    </div>
</form>

<style lang="postcss">
    @reference "tailwindcss";
    :global(html) {
        background-color: theme(--color-gray-100);
    }
</style>
