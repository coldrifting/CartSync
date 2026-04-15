<script lang="ts">
	import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
	import Header from "$lib/components/nav/Header.svelte";
    import {del, get, mutate, patch, post} from "$lib/functions/requests.js";
    import {page} from "$app/state";
    import {createQuery, useQueryClient} from "@tanstack/svelte-query";
    import type Store from "$lib/models/Store.ts";
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import type Aisle from "$lib/models/Aisle.ts";
    
    const client = useQueryClient()
    
    const queryStores = createQuery(() => ({
        queryKey: ['stores'],
        queryFn: () => get<Store[]>('/api/stores', fetch),
    }))
    
    const queryAisles = createQuery(() => ({
        queryKey: ['stores', page.params.storeId, 'aisles'],
        queryFn: () => get<Aisle[]>('/api/aisles/selected', fetch),
    }))

    function getAislesSortData(aisles: Aisle[]) {
        return aisles.map(a => {
            return {
                id: a.id,
                name: a.name,
                subtitle: (a.sortOrder + 1).toString(),
                actionRight: {
                    label: "Edit",
                    icon: "fa-pencil",
                    color: "success",
                    action: () => renameDialog.show(a.id, a.name, true)
                }
            } as SortableItem;
        });
    }
    
    const headerActions: HeaderAction[] = [
        {label: "Add", icon: "fa-plus", color: 'primary', action: () => addDialog.show()}
    ];
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
	
	async function onAdd(value: string) {
		await post(`/api/aisles/add?storeId=${page.params.storeId}`, {name: value});
        await client.invalidateQueries({queryKey: ['stores', page.params.storeId, 'aisles']})
	}
	
	async function onRename(id: string, value: string) {
		await patch(`/api/aisles/${id}/edit`, {"/Name": value});
        await client.invalidateQueries({queryKey: ['stores', page.params.storeId, 'aisles']})
	}
	
	async function onDelete(id: string) {
		await del(`/api/aisles/${id}/delete`);
        await client.invalidateQueries({queryKey: ['stores', page.params.storeId, 'aisles']})
	}
    
    const aisleOrderMutate = mutate<[string, number], Aisle[]>(
        client, 
        ['stores', page.params.storeId, 'aisles'],
        ([id, sortOrder]) => patch(`/api/aisles/${id}/edit`, {"/SortOrder": sortOrder}),
        (query, [id, sortOrder]) => {
            const clone = structuredClone(query);
            
            const currentIndex = query.map(a => a.id).indexOf(id);
            const newIndex = sortOrder;
            
            let item = clone.splice(currentIndex, 1)[0];
            clone.splice(newIndex, 0, item);
            
            for (let i = 0; i < clone.length; i++) {
                clone[i].sortOrder = i;
            }
            
            return clone;
        }
    );
	
    async function onReorder(id: string, newIndex: number) {
		aisleOrderMutate.mutate([id, newIndex]);
    }
</script>

<svelte:head>
    {#if queryStores.isSuccess}
        {@const selectedStore = queryStores.data?.find(s => s.isSelected)}
        <title>{selectedStore?.name} - Aisles</title>
    {/if}
</svelte:head>

<ModalAdd bind:this={addDialog} type="Aisle" addAction={onAdd} scrollOnAdd={true} />
<ModalRename bind:this={renameDialog} type="Aisle" renameAction={onRename} deleteAction={onDelete} warning="All item locations for this aisle will be deleted!"/>

{#if queryStores.isLoading || queryAisles.isLoading}
    <Header back={['/stores', 'Stores']} title="Aisles" subtitle="Aisles" headerActions={headerActions}/>
    <LoadingPage/>
{:else if queryStores.isError || queryAisles.isError}
    <Header back={['/stores', 'Stores']} title="Aisles" subtitle="Aisles" headerActions={headerActions}/>
    <p>ErrorStores: {queryStores.error?.message}</p>
    <p>ErrorAisles: {queryAisles.error?.message}</p>
{:else if queryStores.isSuccess && queryAisles.isSuccess}
    {@const selectedStore = queryStores.data?.find(s => s.isSelected)}
    <Header back={['/stores', 'Stores']} title={selectedStore?.name ?? "Aisles"} subtitle="Aisles"
            headerActions={headerActions}/>

    <ReorderableList listName='list' items={getAislesSortData(queryAisles.data)} onReorder={onReorder}/>
{/if}
