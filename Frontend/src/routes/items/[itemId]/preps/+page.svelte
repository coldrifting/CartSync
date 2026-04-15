<script lang="ts">
    import {createQuery, useQueryClient} from '@tanstack/svelte-query'
    import LoadingPage from "$lib/components/LoadingPage.svelte";
    import {del, get, post, put, mutate, patch} from "$lib/functions/requests.js";
    import {page} from '$app/state'
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";
    import type ItemPrepDetails from "$lib/models/ItemPrepDetails.ts";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import PrepUsagesReport from "$lib/models/PrepUsagesReport.js";

    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const client = useQueryClient()
    const queryKey = ['items', page.params.itemId, 'preps'];

    const headerActions: HeaderAction[] = [
        {
            label: "Add", icon: "fa-plus", color: 'primary', action: () => addDialog.show()
        }
    ];

    const query = createQuery(() => ({
        queryKey: queryKey,
        queryFn: () => get<ItemPrepDetails>(`/api/items/${page.params.itemId}/preps`),
    }))
    
    const addPrepMutation = mutate<string, ItemPrepDetails>(client, queryKey, (value) => post('/api/preps/add', {name: value}));
    
    const renamePrepMutation = mutate<[string, string], ItemPrepDetails>(client, queryKey, ([id, value]) => 
        patch(`/api/preps/${id}/edit`, {"/Name": value}),
        (query, [id, value]) => {
            const index = query.allPreps.map(p => p.id).indexOf(id);
            if (index !== -1) {
                let arr = structuredClone(query.allPreps);
                arr[index].name = value;
                
                return {
                    item: query.item,
                    allPreps: arr
                }
                
            }
            return query;
        }
    );
    
    const removePrepMutation = mutate<string, ItemPrepDetails>(client, queryKey, (value) => 
        del(`/api/preps/${value}/delete`),
        (query, id) => {
            const index = query.allPreps.map(p => p.id).indexOf(id);
            if (index !== -1) {
                return {
                    item: query.item,
                    allPreps: query.allPreps.toSpliced(index, 1)
                }
            }
            return query;
        }
    );
    
	let timer: number;
    function debounce(prepIds: string[]) {
        clearTimeout(timer);
        timer = setTimeout(async () => {
            await put(`/api/items/${page.params.itemId}/preps`, {prepIds: prepIds});
            await client.invalidateQueries({ queryKey: ['items', page.params.itemId, 'preps'] });
            await client.invalidateQueries({ queryKey: ['items', page.params.itemId] });
        }, 500);
    }
    
    async function onAdd(value: string) {
        addPrepMutation.mutate(value)
    }

    async function onRename(id: string, value: string) {
        renamePrepMutation.mutate([id, value])
    }
    
    async function onDelete(id: string) {
        removePrepMutation.mutate(id);
    }
    
    async function onTryDelete(id: string): Promise<Record<string, string[]>> {
        const usages = await get<PrepUsagesReport>(`/api/preps/${id}/usages`);

        let isPrepOnlyUsedByCurrentItem = 
            usages.recipes.length == 0 &&
            usages.items.length == 1 &&
            usages.items[0].id === page.params.itemId;
        
        if (isPrepOnlyUsedByCurrentItem) {
            usages.items = [];
        }
        
        return PrepUsagesReport.getUsages(usages);
    }
    
    async function onValueChange(id: string, value: boolean) {
        if (!query.data) {
            return;
        }
        
        let index = query.data.allPreps.map(p => p.id).indexOf(id);
        if (index === -1) {
            return;
        }
        
        query.data.allPreps[index].isSelected = value;
        let prepIds = query.data.allPreps.filter(p => p.isSelected).map(p => p.id);
        debounce(prepIds);
    }
</script>

<ModalAdd bind:this={addDialog} type="Prep" addAction={onAdd}/>
<ModalRename bind:this={renameDialog} type="Prep" renameAction={onRename} deleteAction={onDelete} tryDeleteAction={onTryDelete}/>

{#if query.isLoading}
<Header back={[`/items/${page.params.itemId}`, 'Item']}
        title="Prep Details"
        headerActions={headerActions}/>
    <LoadingPage/>
{:else if query.isError}
    <p>Error: {query.error.message}</p>
{:else if query.isSuccess}
<Header back={[`/items/${page.params.itemId}`, 'Item']}
        title={query.data.item.name}
        headerActions={headerActions}/>
    <h4>Selected Preps</h4>
    <ul>
        {#each query.data.allPreps as prep}
            <ListItemCheckbox
                    id={prep.id}
                    label={prep.name}
                    name="selectedPrepIds"
                    checked={prep.isSelected}
                    actionRight={{
                        label: 'Edit', 
                        icon: 'fa-pencil', 
                        color: 'success', 
                        action: () => renameDialog.show(prep.id, prep.name, true)
                     }}
                    onValueChange={onValueChange}
            />
        {/each}
    </ul>
{/if}