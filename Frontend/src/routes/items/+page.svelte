<script lang="ts">
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import ItemUsagesReport from "$lib/models/ItemUsagesReport.js";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import {del, get, patch, post} from "$lib/functions/requests.js";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import type Store from "$lib/models/Store.ts";

    const client = useQueryClient()
    
    const queryStores = createQuery(() => ({
        queryKey: ['stores'],
        queryFn: () => get<Store[]>('/api/stores', fetch),
    }))
    
    const queryItems = createQuery(() => ({
        queryKey: ['items'],
        queryFn: () => get<ItemDetails[]>(`/api/items`, fetch),
    }))
    
    let filterText: string = $state('');
    let filter = (items: ItemDetails[]) => {
        if (!filterText) return items;
        let searchText = filterText.toLowerCase().trim();
        return items.filter(item => item.name.toLowerCase().includes(searchText));
    }

    let addDialog: ModalAdd
    let renameDialog: ModalRename

    const headerActions: HeaderAction[] = [{ label: "Add", icon: "fa-plus", color: 'primary', action: () => { addDialog.show() }}];
    
    async function onAdd(value: string) {
        await post('/api/items/add', {name: value});
        await client.invalidateQueries({queryKey: ['items']})
    }
    
    async function onRename(id: string, value: string) {
        await patch(`/api/items/${id}/edit`, {"/Name": value});
        await client.invalidateQueries({queryKey: ['items']})
        await client.invalidateQueries({queryKey: ['items', id]})
    }
    
    async function onDelete(id: string) {
        await del(`/api/items/${id}/delete`);
        await client.invalidateQueries({queryKey: ['items']})
    }
    
    async function onTryDelete(id: string): Promise<Record<string, string[]>> {
        const usages = await get<ItemUsagesReport>(`/api/items/${id}/usages`);
        return ItemUsagesReport.getUsages(usages);
    }
</script>

<svelte:head>
    <title>CartSync - Items</title>
</svelte:head>

<Header title="Items" headerActions={headerActions} bind:filterText={filterText}/>

<ModalAdd bind:this={addDialog} type="Item" addAction={onAdd}/>
<ModalRename bind:this={renameDialog} type="Item" renameAction={onRename} deleteAction={onDelete} tryDeleteAction={onTryDelete} />

{#if queryStores.isLoading || queryItems.isLoading}
    <LoadingPage/>
{:else if queryStores.isError || queryItems.isError}
    <p>ErrorStore: {queryStores.error?.message}</p>
    <p>ErrorItems: {queryItems.error?.message}</p>
{:else if queryStores.isSuccess && queryItems.isSuccess}
    {@const filteredItems = filter(queryItems.data)}
    {@const storeId = queryStores.data.find(s => s.isSelected)?.id ?? ""}
    <ul>
        {#each filteredItems as item}
            <ListItemLink label={item.name}
                             info={item.locations.find(a => a.storeId === storeId)?.aisleName ?? "(Not Set)"}
                             href="/items/{item.id}"
                             actionRight={{
                                label: 'Edit', 
                                icon: 'fa-pencil', 
                                color: 'success', 
                                action: () => renameDialog.show(item.id, item.name, true)
                             }}
            />
        {/each}
    </ul>
{/if}