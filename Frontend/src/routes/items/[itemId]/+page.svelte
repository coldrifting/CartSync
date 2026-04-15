<script lang="ts">
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import SelectDefaultUnits from "$lib/components/item/SelectDefaultUnits.svelte";
    import SelectLocation from "$lib/components/item/SelectLocation.svelte";
    import SelectTemp from "$lib/components/item/SelectTemp.svelte";
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import {createQuery} from "@tanstack/svelte-query";
    import {get} from "$lib/functions/requests.js";
    import {page} from "$app/state";
    import type Store from "$lib/models/Store.ts";
    import type Aisle from "$lib/models/Aisle.ts";

    function getPrepsString(item: ItemDetails): string {
        return item.preps
            .map(prep => prep.name)
            .slice(0, 3)
            .join(", ") + (item.preps.length > 3 ? ", ..." : "");
    }
    
    const queryStores = createQuery(() => ({
        queryKey: ['stores'],
        queryFn: () => get<Store[]>('/api/stores', fetch),
    }))
    
    const queryAisles = createQuery(() => ({
        queryKey: ['aisles'],
        queryFn: () => get<Aisle[]>('/api/aisles', fetch),
    }))
    
    const queryItem = createQuery(() => ({
        queryKey: ['items', page.params.itemId],
        queryFn: () => get<ItemDetails>(`/api/items/${page.params.itemId}`, fetch),
    }))
    
    
</script>

<svelte:head>
    {#if queryItem.isSuccess}
        <title>{queryItem.data.name}</title>
    {/if}
</svelte:head>

{#if queryStores.isLoading || queryAisles.isLoading || queryItem.isLoading}
<Header back={[`/items/`, 'Item']}
        title="Item Details"
        headerActions={[]}/>
    <LoadingPage/>
{:else if queryStores.isError || queryAisles.isError || queryItem.isError}
    <p>ErrorStore: {queryStores.error?.message}</p>
    <p>ErrorAisles: {queryAisles.error?.message}</p>
    <p>ErrorItem: {queryItem.error?.message}</p>
{:else if queryStores.isSuccess && queryAisles.isSuccess && queryItem.isSuccess}
    {@const prepsString = getPrepsString(queryItem.data)}
    <Header back={['/items', 'Items']} title={queryItem.data.name}/>

    <h4>Details</h4>
    
    <SelectTemp itemId={queryItem.data.id} itemTemp={queryItem.data.temp} />
    <SelectDefaultUnits itemId={queryItem.data.id} itemDefaultUnitType={queryItem.data.defaultUnitType} />

    <h4>Preps</h4>
    <div>
        <ListItemLink label={prepsString === "" ? "(None)" : prepsString}
                      showArrow={true}
                      href="/items/{queryItem.data.id}/preps"
                      info="Edit"/>
    </div>

    <h4>Location</h4>
    <SelectLocation aisles={queryAisles.data} 
                    stores={queryStores.data} 
                    item={queryItem.data}/>

{/if}