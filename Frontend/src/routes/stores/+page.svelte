<script lang="ts">
    import {invalidateAll} from "$app/navigation";
    import {patch, post, del, put} from "$lib/functions/requests.js";
    import type {PageProps} from './$types';
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";
    import ListItemRadio from "$lib/components/lists/ListItemRadio.svelte";
    
    let {data}: PageProps = $props();
    
    let stores = $derived(data.stores);
    let selectedStoreName = $derived(data.selectedStore.name);
    let selectedStoreId = $derived(data.selectedStore.id);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const headerActions: HeaderAction[] = [{label: "Add Store", icon: "fa-plus", action: () => {addDialog.show()}}];
    
    async function addAction(value: string) {
        await post("/api/stores/add", {name: value});
    }
    
    async function renameAction(id: string, value: string) {
        await patch(`/api/stores/${id}/edit`, {"/Name": value});
    }
    
    async function deleteAction(id: string) {
        await del(`/api/stores/${id}/delete`);
    }
    
    async function onChange(value: string) {
        await put(`/api/stores/${value}/select`, {});
        await invalidateAll();
    }
</script>

<svelte:head>
    <title>Stores</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Store" addAction={addAction}/>
<ModalRename bind:this={renameDialog} type="Store" renameAction={renameAction} deleteAction={deleteAction} warning="All item locations for [Name] will be deleted!"/>

<Header title="Stores" headerActions={headerActions} />

<h4>Selected Store</h4>
<div>
    <ul>
        <ListItemLink label={selectedStoreName} 
                         href="/stores/{selectedStoreId}" 
                         info="Aisles" 
                         showArrow={true}/>
    </ul>
</div>

<h4>All Stores</h4>
<ul>
    {#each stores as store}
        <ListItemRadio
                id={store.id}
                label={store.name}
                group="selectedStoreId"
                selectedValue={selectedStoreId}
                actionRight={{
                    label: 'Edit', 
                    icon: 'fa-pencil', 
                    color: 'success', 
                    action: () => renameDialog.show(store.id, store.name, !store.isSelected)
                }}
                onValueChange={onChange}
        />
    {/each}
</ul>