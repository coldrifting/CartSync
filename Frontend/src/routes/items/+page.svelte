<script lang="ts">
    import type {PageProps} from './$types';
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import ItemUsagesReport from "$lib/models/ItemUsagesReport.js";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import {del, get, patch, post} from "$lib/functions/requests.js";

    let {data}: PageProps = $props();

    let filterText: string = $state('');
    let filter = (items: ItemDetails[]) => {
        if (!filterText) return items;
        let searchText = filterText.toLowerCase().trim();
        return items.filter(item => item.name.toLowerCase().includes(searchText));
    }

    let filteredIngredients: ItemDetails[] = $derived(filter(data.ingredients));

    let addDialog: ModalAdd
    let renameDialog: ModalRename

    const headerActions: HeaderAction[] = [{ label: "Add Item", icon: "fa-plus", action: () => { addDialog.show() }}];
    
    async function onAdd(value: string) {
        await post('/api/items/add', {name: value});
    }
    
    async function onRename(id: string, value: string) {
        await patch(`/api/items/${id}/edit`, {"/Name": value});
    }
    
    async function onDelete(id: string) {
        await del(`/api/items/${id}/delete`);
    }
    
    async function onTryDelete(id: string): Promise<Record<string, string[]>> {
        const usages = await get<ItemUsagesReport>(`/api/items/${id}/usages`);
        return ItemUsagesReport.getUsages(usages);
    }
</script>

<svelte:head>
    <title>Ingredients</title>
</svelte:head>

<Header title="Items" headerActions={headerActions} bind:filterText={filterText}/>

<ModalAdd bind:this={addDialog} type="Item" addAction={onAdd}/>
<ModalRename bind:this={renameDialog} type="Item" renameAction={onRename} deleteAction={onDelete} tryDeleteAction={onTryDelete} />

<ul>
    {#each filteredIngredients as ingredient}
        <ListItemLink label={ingredient.name}
                         info={ingredient.location?.aisleName ?? "(Not Set)"}
                         href="/items/{ingredient.id}"
                         actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => renameDialog.show(ingredient.id, ingredient.name, true)
                         }}
        />
    {/each}
</ul>