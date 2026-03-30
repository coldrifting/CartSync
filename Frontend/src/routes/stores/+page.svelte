<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ListItem from "$lib/components/ListItem.svelte";
    import ListRadioButton from "$lib/components/ListRadioButton.svelte";
    import {Button, FormGroup, Input, Modal, ModalFooter} from "@sveltestrap/sveltestrap";
    let {data}: PageProps = $props();
    
    let stores = $derived(data.stores);
    let selectedStore = $derived(data.selectedStore);
    let selectedStoreId = $derived(data.selectedStore.storeId);
    
    let selectStoreForm: HTMLFormElement;
    
    let deleteId = $state('');
    let renameId = $state('');
    let newName = $state('');
        
    let showAddDialog = $state(false);
    const toggleAddDialog = () => (showAddDialog = !showAddDialog);
    const closeAddDialog = () => (showAddDialog = false);

    let showRenameDialog = $state(false);
    const toggleRenameDialog = () => (showRenameDialog = !showRenameDialog);
    const closeRenameDialog = () => (showRenameDialog = false);

    let showDeleteDialog = $state(false);
    const toggleDeleteDialog = () => {
        if (showDeleteDialog) {
            deleteId = "";
        }
        return (showDeleteDialog = !showDeleteDialog);
    };
    const closeDeleteDialog = () => {
        deleteId = "";
        return (showDeleteDialog = false);
    };
    
    let actions: ContextAction[] = [
        {
            label: "Rename",
            isDestructive: false,
            action: (val, name) => {
                renameId = val;
                newName = name ?? "";
                showRenameDialog = true;
            }
        },
        {
            label: "Delete",
            isDestructive: true,
            action: (val, _) => {
                deleteId = val;
                showDeleteDialog = true;
            }
        }
    ];
</script>

<svelte:head>
    <title>Stores</title>
</svelte:head>

<Modal body header="Add Store"
       isOpen={showAddDialog}
       toggle={toggleAddDialog}
       centered={true}>
    <form method="POST"
          action="?/addStore"
          id="addForm"
          use:enhance={() => {closeAddDialog()}}>
        <div>
            <FormGroup floating label="Store Name">
                <Input name="storeName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeAddDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Rename Store"
       isOpen={showRenameDialog}
       toggle={toggleRenameDialog}
       centered={true}>
    <form method="POST"
          action="?/renameStore"
          id="renameForm"
          use:enhance={() => {closeRenameDialog()}}>
        <div>
            <input hidden name="id" bind:value={renameId}/>
            <FormGroup floating label="Store Name">
                <Input name="storeName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeRenameDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""} >Rename</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Delete Store"
       isOpen={showDeleteDialog}
       toggle={toggleDeleteDialog}
       centered={true}>
    <form method="POST"
          action="?/deleteStore"
          id="deleteForm"
          use:enhance={() => {closeDeleteDialog()}}>
        <div>
            <input hidden name="id" bind:value={deleteId}/>
            <p>All item locations for this store will be deleted.<br>Are you sure?</p>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeDeleteDialog}>Cancel</Button>
            <Button color="danger" type="submit">Delete</Button>
        </ModalFooter>
    </form>
</Modal>

<h1>Stores</h1>

<h4 class="mt-4">Selected Store</h4>
<div>
    <ul>
        <ListItem name={selectedStore.storeName} id="" link="/stores/{selectedStoreId}" subtitle="Edit" actions={[]} isSubtitleActive={true} />
    </ul>
</div>


<form method="POST"
      action="?/selectStore"
      bind:this={selectStoreForm}
      use:enhance>
    <h4 class="mt-4">All Stores</h4>
    <div>
        <ul>
            {#each stores as store}
                <ListRadioButton 
                        name="selectedStoreId"
                        label={store.storeName} 
                        value={store.storeId}
                        actions={actions} 
                        group={selectedStoreId} 
                        onchange={() => selectStoreForm.requestSubmit()}/>
            {/each}
        </ul>
    </div>
</form>

<Button color="primary mt-3 p-2" block onclick={() => {
                newName = "";
                showAddDialog = true;
}}>
    Add Store
</Button>