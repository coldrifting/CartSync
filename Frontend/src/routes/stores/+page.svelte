<script lang="ts">
    import {enhance} from '$app/forms';
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
</script>

<svelte:head>
    <title>Stores</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Store"/>
<ModalRename bind:this={renameDialog} type="Store" warning="All item locations for [Name] will be deleted!"/>

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

<form method="POST"
      action="?/selectStore"
      use:enhance>
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
            />
        {/each}
    </ul>
</form>