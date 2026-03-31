<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ListItem from "$lib/components/ListItem.svelte";
    import ListRadioButton from "$lib/components/ListRadioButton.svelte";
    import {Button} from "@sveltestrap/sveltestrap";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    
    let {data}: PageProps = $props();
    
    let stores = $derived(data.stores);
    let selectedStoreName = $derived(data.selectedStore.storeName);
    let selectedStoreId = $derived(data.selectedStore.storeId);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    let contextActions: ContextAction[] = [
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, _: string | undefined) => {deleteDialog.show(id)} }
    ];
    
    let selectStoreForm: HTMLFormElement;
</script>

<svelte:head>
    <title>Stores</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addStore" header="Add Store" labelAdd="Store Name" />
<ModalRename bind:this={renameDialog} action="renameStore" header="Rename Store" labelRename="Store Name" />
<ModalDelete bind:this={deleteDialog} action="deleteStore" header="Delete Store" warning="All item locations for this store will be deleted!" />

<h1>Stores</h1>

<h4>Selected Store</h4>
<div>
    <ul>
        <ListItem name={selectedStoreName} id="" link="/stores/{selectedStoreId}" subtitle="Edit" contextActions={[]} />
    </ul>
</div>

<form method="POST"
      action="?/selectStore"
      bind:this={selectStoreForm}
      use:enhance>
    <h4>All Stores</h4>
    <ul>
        {#each stores as store}
            <ListRadioButton 
                    name="selectedStoreId"
                    label={store.storeName} 
                    value={store.storeId}
                    contextActions={contextActions.filter(a => a.label !== "Delete" || !store.isSelected)}
                    group={selectedStoreId} 
                    onchange={() => selectStoreForm.requestSubmit()}/>
        {/each}
    </ul>
</form>

<Button color="primary mt-3 p-2" block onclick={() => {addDialog.show()}}>
    Add Store
</Button>