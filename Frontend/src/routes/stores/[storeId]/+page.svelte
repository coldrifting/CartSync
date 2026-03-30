<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
	import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
	import {Button, FormGroup, Input, Modal, ModalFooter} from "@sveltestrap/sveltestrap";
    
    let {data}: PageProps = $props();
	
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
    
    let storeId = $derived(data.store.storeId);
    const storeName = $derived(data.store.storeName);
    let aisles: SortableItem[] = $derived(data.aisles.map(a => {
        return {
            id: a.aisleId,
			name: a.aisleName,
			subtitle: (a.sortOrder + 1).toString(),
			contextActions: actions
        }
    }));
    
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
	
	let reorderState = $state<{id: string, index: number}>({id: "", index: -1})
	let reorderId = $derived(reorderState.id);
	let reorderIndex = $derived(reorderState.index);
	
    let onReorder = (id: string, newIndex: number) => {
		reorderState = {id: id, index: newIndex};
    }
	
	$effect(() => {
		if (reorderState.id != "" && reorderState.index != -1) {
			reorderForm.requestSubmit();
		}
	})
	
	let reorderForm: HTMLFormElement;
    
</script>

<svelte:head>
    <title>{storeName}</title>
</svelte:head>


<form method="POST"
	  action="?/reorderAisle"
	  bind:this={reorderForm}
	  use:enhance>
	<input hidden name="storeId" bind:value={storeId} />
	<input hidden name="aisleId" bind:value={reorderId} />
	<input hidden name="aisleSortOrder" bind:value={reorderIndex} />
</form>

<Modal body header="Add Aisle"
       isOpen={showAddDialog}
       toggle={toggleAddDialog}
       centered={true}>
    <form method="POST"
          action="?/addAisle"
          id="addForm"
          use:enhance={() => {closeAddDialog()}}>
        <div>
            <input hidden name="storeId" bind:value={storeId}/>
            <FormGroup floating label="Aisle Name">
                <Input name="aisleName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeAddDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Rename Aisle"
       isOpen={showRenameDialog}
       toggle={toggleRenameDialog}
       centered={true}>
    <form method="POST"
          action="?/renameAisle"
          id="renameForm"
          use:enhance={() => {closeRenameDialog()}}>
        <div>
            <input hidden name="storeId" bind:value={storeId}/>
            <input hidden name="aisleId" bind:value={renameId}/>
            <FormGroup floating label="Aisle Name">
                <Input name="aisleName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeRenameDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""} >Rename</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Delete Aisle"
       isOpen={showDeleteDialog}
       toggle={toggleDeleteDialog}
       centered={true}>
    <form method="POST"
          action="?/deleteAisle"
          id="deleteForm"
          use:enhance={() => {closeDeleteDialog()}}>
        <div>
            <input hidden name="storeId" bind:value={storeId}/>
            <input hidden name="aisleId" bind:value={deleteId}/>
            <p>All item locations for this aisle will be deleted.<br>Are you sure?</p>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeDeleteDialog}>Cancel</Button>
            <Button color="danger" type="submit">Delete</Button>
        </ModalFooter>
    </form>
</Modal>

<h1>{storeName}</h1>

<h4 class="mt-4">Aisles</h4>
<ReorderableList listName='list' items={aisles} onReorder={onReorder}/>

<Button color="primary mt-3 p-2" block onclick={() => {
                newName = "";
                showAddDialog = true;
}}>
    Add Aisle
</Button>