<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
	import ReorderableList from "$lib/components/dragAndDrop/ReorderableList.svelte";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
	import Header from "$lib/components/nav/Header.svelte";
	import {del, patch, post} from "$lib/functions/requests.js";
    
    let {data}: PageProps = $props();
    
    const storeName = $derived(data.store.name);
    let aisles: SortableItem[] = $derived(data.aisles.map(a => {
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
    }));
	
    const headerActions: HeaderAction[] = [
        {label: "Add Aisle", icon: "fa-plus", action: () => addDialog.show()}
    ];
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
	let reorderForm: HTMLFormElement;
    
	let reorderState = $state<{id: string, index: number}>({id: "", index: -1})
	let reorderId = $derived(reorderState.id);
	let reorderIndex = $derived(reorderState.index);
	
	async function onAdd(value: string) {
		await post(`/api/aisles/add?storeId=${data.store.id}`, {name: value});
	}
	
	async function onRename(id: string, value: string) {
		await patch(`/api/aisles/${id}/edit`, {"/Name": value});
	}
	
	async function onDelete(id: string) {
		await del(`/api/aisles/${id}/delete`);
	}
	
    async function onReorder(id: string, newIndex: number) {
		await patch(`/api/aisles/${id}/edit`, {"/SortOrder": newIndex});
    }
</script>

<svelte:head>
    <title>{storeName}</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Aisle" addAction={onAdd} scrollOnAdd={true} />
<ModalRename bind:this={renameDialog} type="Aisle" renameAction={onRename} deleteAction={onDelete} warning="All item locations for this aisle will be deleted!"/>

<Header back={['/stores', 'Stores']} title={storeName} subtitle="Aisles" headerActions={headerActions}/>

<ReorderableList listName='list' items={aisles} onReorder={onReorder} />
<form method="POST"
	  action="?/reorderAisle"
	  bind:this={reorderForm}
	  use:enhance>
	<input hidden name="id" bind:value={reorderId} />
	<input hidden name="aisleSortOrder" bind:value={reorderIndex} />
</form>