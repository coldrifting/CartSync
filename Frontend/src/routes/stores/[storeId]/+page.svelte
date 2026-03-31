<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
	import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
	import Header from "$lib/components/Header.svelte";
    
    let {data}: PageProps = $props();
    
    const storeName = $derived(data.store.storeName);
    let aisles: SortableItem[] = $derived(data.aisles.map(a => {
        return {
            id: a.aisleId,
			name: a.aisleName,
			subtitle: (a.sortOrder + 1).toString()
        }
    }));
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    let contextActions: ContextAction[] = [
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, _: string | undefined) => {deleteDialog.show(id)} }
    ];
	
	let reorderForm: HTMLFormElement;
    
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
</script>

<svelte:head>
    <title>{storeName}</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addAisle" header="Add Aisle" labelAdd="Aisle Name" scrollOnAdd={true} />
<ModalRename bind:this={renameDialog} action="renameAisle" header="Rename Aisle" labelRename="Aisle Name" />
<ModalDelete bind:this={deleteDialog} action="deleteAisle" header="Delete Aisle" warning="All item locations for this aisle will be deleted!" />

<Header back={['/stores', 'Stores']} title={storeName} subtitle="Aisles" actions={[{label: "Add Aisle", icon: "fa-plus", action: () => {addDialog.show()}}]} />

<ReorderableList listName='list' items={aisles} onReorder={onReorder} contextActions={contextActions} />
<form method="POST"
	  action="?/reorderAisle"
	  bind:this={reorderForm}
	  use:enhance>
	<input hidden name="id" bind:value={reorderId} />
	<input hidden name="aisleSortOrder" bind:value={reorderIndex} />
</form>