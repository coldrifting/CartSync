<script lang="ts">
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import Amounts from "$lib/models/AmountGroup.js";
    import {put} from "$lib/functions/requests.js";
    let {data}: PageProps = $props();
    
    let storeName = $derived(data.cart.storeName);
    let aisles = $derived(data.cart.aisles);
    
    const onchange = async (id: string, value: boolean) => {
        await put(`/api/cart/entries/${id}/edit?isChecked=${value}`, {});
    }
</script>

<svelte:head>
    <title>Cart</title>
</svelte:head>

<Header title='Cart List' back={['/cart','Cart']} subtitle={storeName}/>

{#each aisles as aisle}
    <h5 class="mt-3">{aisle.aisleName}</h5>
    <ul>
        {#each aisle.items as item}
            <ListItemCheckbox id={item.entryId}
                              label={item.item.name}
                              name='id'
                              checked={item.isChecked}
                              info={Amounts.asString(item.amounts)}
                              subInfo={item.prep?.name}
                              isSingle={true}
                              onValueChange={onchange}
            />
        {/each}
    </ul>
{/each}