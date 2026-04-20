<script lang="ts">
    import ListItemButton from "$lib/components/lists/ListItemButton.svelte";
    import Amount from "$lib/models/Amount.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ModalCartAdd from "$lib/components/modal/cart/ModalCartAdd.svelte";
    import ModalCartEditRecipe from "$lib/components/modal/cart/ModalCartEditRecipe.svelte";
    import ModalCartEditItem from "$lib/components/modal/cart/ModalCartEditItem.svelte";
    import {goto} from "$app/navigation";
    import {get, post} from "$lib/functions/requests.js";
    import {browser} from "$app/environment";
    import {redirect} from "@sveltejs/kit";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import ModalLoading from "$lib/components/modal/ModalLoading.svelte";
    import {createQuery} from "@tanstack/svelte-query";
    import type CartSelect from "$lib/models/CartSelect.ts";
    
    const queryCart = createQuery(() => ({
        queryKey: ['cart'],
        queryFn: () => get<CartSelect>('/api/cart/selection', fetch)
    }))
    
    let isCartGenerating: boolean = $state(false);
    const headerActions: HeaderAction[] = [
        {label: "Add", icon: "fa-plus", color: 'primary', action: async () => { modalCartAdd?.show() }},
        {label: "Cart", icon: "fa-shopping-cart", color: 'success', action: async () => { 
            isCartGenerating = true;
            try {
                if (queryCart.isSuccess && queryCart.data.cartLastGeneratedTime < queryCart.data.cartSelectionLastUpdatedTime) {
                    await post('/api/cart/generate', {});
                }
                    
                if (browser) {
                    await goto('/cart/list');
                }
                else {
                    redirect(307, '/cart/list');
                }
            }
            catch(error) {
                console.error(error);
            }
            isCartGenerating = false;
        }}
    ];
    
    let modalCartAdd: ModalCartAdd | undefined = $state(undefined);
    let modalCartEditRecipe: ModalCartEditRecipe | undefined = $state(undefined);
    let modalCartEditItem: ModalCartEditItem | undefined = $state(undefined);
    
</script>

<svelte:head>
    <title>CartSync - Cart</title>
</svelte:head>

<Header title="Cart Setup" headerActions={headerActions}/>

{#if isCartGenerating}
<ModalLoading title="Generating Cart..."></ModalLoading>
{/if}

{#if queryCart.isLoading}
    <LoadingPage/>
{:else if queryCart.isError}
    <p>Error: {queryCart.error?.message}</p>
{:else if queryCart.isSuccess}
    <ModalCartAdd bind:this={modalCartAdd}
                  remainingRecipes={queryCart.data.remainingRecipes}
                  remainingItems={queryCart.data.remainingItems}/>
    
    <ModalCartEditRecipe bind:this={modalCartEditRecipe} cartRecipes={queryCart.data.recipes}/>
    <ModalCartEditItem bind:this={modalCartEditItem} cartItems={queryCart.data.items}/>
    
    {#if queryCart.data.recipes.length > 0}
        <h4>Recipes</h4>
        <ul>
            {#each queryCart.data.recipes as recipe}
                <ListItemButton label={recipe.name} 
                                info="Qty: {recipe.quantity.toFixed(0)}" 
                                action={() => {modalCartEditRecipe?.show(recipe.id)}}/>
            {/each}
        </ul>
    {/if}
    
    {#if queryCart.data.items.length > 0}
        <h4>Items</h4>
        <ul>
            {#each queryCart.data.items as item}
                <ListItemButton label={item.item.name}
                                info={Amount.asString(item.amount)}
                                subInfo={item.prep?.name}
                                action={() => {modalCartEditItem?.show(item.item.id, item.prep?.id ?? null)}}/>
            {/each}
        </ul>
    {/if}
{/if}