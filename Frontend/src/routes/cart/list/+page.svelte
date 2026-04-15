<script lang="ts">
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import Amounts from "$lib/models/AmountGroup.js";
    import {get, put} from "$lib/functions/requests.js";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import type CartResult from "$lib/models/CartResult.ts";
    
    const client = useQueryClient()
    
    const queryCart = createQuery(() => ({
        queryKey: ['cart', 'list'],
        queryFn: () => get<CartResult>("/api/cart", fetch)
    }))
    
    const onchange = async (id: string, value: boolean) => {
        await put(`/api/cart/entries/${id}/edit?isChecked=${value}`, {});
        await client.invalidateQueries({queryKey: ['cart', 'list']})
    }
</script>

<svelte:head>
    <title>Cart</title>
</svelte:head>

<Header title='Cart List' back={['/cart','Cart']}/>
{#if queryCart.isLoading}
    <LoadingPage/>
{:else if queryCart.isError}
    <p>Error: {queryCart.error?.message}</p>
{:else if queryCart.isSuccess}
    {#each queryCart.data.aisles as aisle}
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
{/if}
