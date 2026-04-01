<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import Header from "$lib/components/Header.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";
    import ListElementRadio from "$lib/components/ListElementRadio.svelte";
    
    let {data}: PageProps = $props();
    
    let stores = $derived(data.stores);
    let selectedStoreName = $derived(data.selectedStore.storeName);
    let selectedStoreId = $derived(data.selectedStore.storeId);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    let contextActions: ContextAction[] = [
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, value: string | undefined) => {deleteDialog.show(id, value)} }
    ];
    
    let selectStoreForm: HTMLFormElement;
    
    const headerActions: HeaderAction[] = [
        {label: "Add Store", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
</script>

<svelte:head>
    <title>Stores</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addStore" header="Add Store" labelAdd="Store Name" />
<ModalRename bind:this={renameDialog} action="renameStore" header="Rename Store" labelRename="Store Name" />
<ModalDelete bind:this={deleteDialog} action="deleteStore" header="Delete Store" warning="All item locations for [Name] will be deleted!" />

<Header title="Stores" headerActions={headerActions} />

<h4>Selected Store</h4>
<div>
    <ul>
        <ListElementLink id={selectedStoreId} label={selectedStoreName} link="/stores/{selectedStoreId}" info="Aisles"/>
    </ul>
</div>

<form method="POST"
      action="?/selectStore"
      bind:this={selectStoreForm}
      use:enhance>
    <h4>All Stores</h4>
    <ul>
        {#each stores as store}
            <ListElementRadio 
                    id={store.storeId}
                    label={store.storeName} 
                    contextActions={contextActions.filter(a => a.label !== "Delete" || !store.isSelected)}
                    group="selectedStoreId"
                    selectedValue={selectedStoreId} />
        {/each}
    </ul>
</form>