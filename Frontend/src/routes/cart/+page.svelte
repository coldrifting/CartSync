<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ListItemButton from "$lib/components/lists/ListItemButton.svelte";
    import Amount from "$lib/models/Amount.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ModalCartAdd from "$lib/components/modal/cart/ModalCartAdd.svelte";
    import ModalCartEditRecipe from "$lib/components/modal/cart/ModalCartEditRecipe.svelte";
    import ModalCartEditItem from "$lib/components/modal/cart/ModalCartEditItem.svelte";
    import type CartSelectItem from "$lib/models/CartSelectItem.ts";
    let {data}: PageProps = $props();
    
    const headerActions: HeaderAction[] = [
        {label: "Add", icon: "fa-plus", action: () => { modalCartAdd.show() }},
        {label: "Generate", icon: "fa-refresh", action: () => { generateCartForm.requestSubmit() }}
    ];

    let recipeContextActions: ContextAction[] = [
        {
            label: "Edit", action: (id: string, _: string | undefined) => {
                modalCartEditRecipe.show(id)
            }
        },
        {
            label: "Remove", action: (id: string, _: string | undefined) => {
                deleteRecipeId = id;
                tick().then(() => deleteRecipeForm.requestSubmit());
            }
        },
    ]

    let itemContextActions: ContextAction[] = [
        {
            label: "Edit", action: (id: string, _: string | undefined) => {
                let ids = extractId(id);
                modalCartEditItem.show(ids.itemId, ids.prepId ?? null)
            }
        },
        {
            label: "Remove", action: (id: string, _: string | undefined) => {
                let ids = extractId(id);
                deleteItemId = ids.itemId;
                deletePrepId = ids.prepId ?? '';
                tick().then(() => deleteItemForm.requestSubmit());
            }
        },
    ]
    
    const mergeId = (item: CartSelectItem) => {
        return item.item.id + '/' + (item.prep?.id ?? '(None)');
    }
    
    const extractId = (id: string) => {
        return {itemId: id.split('/')[0], prepId: id.split('/')[1] === '(None)' ? undefined : id.split('/')[1]}
    }
    
    let modalCartAdd: ModalCartAdd;
    let modalCartEditRecipe: ModalCartEditRecipe;
    let modalCartEditItem: ModalCartEditItem;
    
    let generateCartForm: HTMLFormElement;
    
    let deleteRecipeForm: HTMLFormElement;
    let deleteRecipeId: string = $state('');
    
    let deleteItemForm: HTMLFormElement;
    let deleteItemId: string = $state('');
    let deletePrepId: string = $state('');
</script>

<svelte:head>
    <title>Cart</title>
</svelte:head>

<ModalCartAdd bind:this={modalCartAdd}
              formUrl='addCartItemOrRecipe'
              remainingRecipes={data.remainingRecipes}
              remainingItems={data.remainingItems}/>

<ModalCartEditRecipe bind:this={modalCartEditRecipe}
              formUrl='editCartRecipe'
              cartRecipes={data.recipes}/>

<ModalCartEditItem bind:this={modalCartEditItem}
              formUrl='editCartItem'
              cartItems={data.items}/>

<Header title="Cart Selection" headerActions={headerActions}/>

{#if data.recipes.length > 0}
    <h4>Recipes</h4>
    <ul>
        {#each data.recipes as recipe}
            <ListItemButton id={recipe.id} 
                               label={recipe.name} 
                               info="Qty: {recipe.quantity.toFixed(0)}" 
                               action={() => {modalCartEditRecipe.show(recipe.id)}}/>
        {/each}
    </ul>
{/if}

{#if data.items.length > 0}
    <h4>Items</h4>
    <ul>
        {#each data.items as item}
            <ListItemButton label={item.item.name}
                            info={Amount.asString(item.amount)}
                            subInfo={item.prep?.name}
                            action={() => {modalCartEditItem.show(item.item.id, item.prep?.id ?? null)}}/>
        {/each}
    </ul>
{/if}

<form method="POST"
      action="?/removeCartRecipe"
      bind:this={deleteRecipeForm}
      use:enhance>
    <input name="recipeId" bind:value={deleteRecipeId} hidden required/>
    <input hidden type="submit"/>
</form>

<form method="POST"
      action="?/removeCartItem"
      bind:this={deleteItemForm}
      use:enhance>
    <input name="itemId" bind:value={deleteItemId} hidden required/>
    <input name="prepId" bind:value={deletePrepId} hidden/>
    <input hidden type="submit"/>
</form>

<form method="POST"
      action="?/generateCart"
      bind:this={generateCartForm}
      use:enhance>
    <input hidden type="submit"/>
</form>