<script lang="ts">
    import type {PageProps} from './$types';
    import ListItemButton from "$lib/components/lists/ListItemButton.svelte";
    import Amount from "$lib/models/Amount.js";
    import Header from "$lib/components/nav/Header.svelte";
    import ModalCartAdd from "$lib/components/modal/cart/ModalCartAdd.svelte";
    import ModalCartEditRecipe from "$lib/components/modal/cart/ModalCartEditRecipe.svelte";
    import ModalCartEditItem from "$lib/components/modal/cart/ModalCartEditItem.svelte";
    import {goto} from "$app/navigation";
    import {post} from "$lib/functions/requests.js";
    import {browser} from "$app/environment";
    import {redirect} from "@sveltejs/kit";
    
    let {data}: PageProps = $props();
    
    const headerActions: HeaderAction[] = [
        {label: "Add", icon: "fa-plus", color: 'primary', action: () => { modalCartAdd.show() }},
        {label: "Generate", icon: "fa-refresh", color: 'success', action: async () => { 
            try {
                await post('/api/cart/generate', {});
                    
                if (browser) {
                    await goto('/cart/list');
                }
                else {
                    redirect(307, '/cart/list');
                }
            }
            catch {
                console.log('Error generating cart');
            }
        }}
    ];
    
    let modalCartAdd: ModalCartAdd;
    let modalCartEditRecipe: ModalCartEditRecipe;
    let modalCartEditItem: ModalCartEditItem;
</script>

<svelte:head>
    <title>CartSync - Cart</title>
</svelte:head>

<ModalCartAdd bind:this={modalCartAdd}
              remainingRecipes={data.remainingRecipes}
              remainingItems={data.remainingItems}/>

<ModalCartEditRecipe bind:this={modalCartEditRecipe} cartRecipes={data.recipes}/>
<ModalCartEditItem bind:this={modalCartEditItem} cartItems={data.items}/>

<Header title="Cart" headerActions={headerActions}/>

{#if data.recipes.length > 0}
    <h4>Recipes</h4>
    <ul>
        {#each data.recipes as recipe}
            <ListItemButton label={recipe.name} 
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