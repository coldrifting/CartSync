<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import Amounts from "$lib/models/AmountGroup.js";
    let {data}: PageProps = $props();
    
    let storeName = $derived(data.cart.storeName);
    let aisles = $derived(data.cart.aisles);
</script>

<svelte:head>
    <title>Cart</title>
</svelte:head>


<Header title='Cart' back={['/cart','Cart Selection']} subtitle={storeName}/>

{#each aisles as aisle}
    <h5 class="mt-3">{aisle.aisleName}</h5>
    <ul>
        {#each aisle.items as item}
            <form method="POST"
                  action="?/toggleCartItemChecked"
                  use:enhance>
                <ListItemCheckbox id={item.item.id + '/' + (item.prep?.id ?? '(None)')}
                                     label={item.item.name}
                                     name='id'
                                     checked={item.isChecked}
                                     info={Amounts.asString(item.amounts)}
                                     subInfo={item.prep?.name}
                                     isSingle={true}
                />
            </form>
        {/each}
    </ul>
{/each}