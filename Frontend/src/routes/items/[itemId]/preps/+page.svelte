<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemCheckbox from "$lib/components/lists/ListItemCheckbox.svelte";

    let {data}: PageProps = $props();
    let itemId: string = $derived(data.item.id);
    
    let preps = $derived(data.preps);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    
    const headerActions: HeaderAction[] = [
        {label: "Add Prep", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
</script>

<svelte:head>
    <title>{data.item.name} - Preps</title>
</svelte:head>

<ModalAdd bind:this={addDialog} type="Prep"/>
<ModalRename bind:this={renameDialog} type="Prep" tryDelete={true}/>

<Header back={[`/items/${itemId}`, 'Item']} 
        title={data.item.name} 
        subtitle="Preps" 
        headerActions={headerActions} />

<form method="POST"
      action="?/editPreps"
      id="editPrepForm"
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
    <ul>
        {#each preps as prep}
            <ListItemCheckbox
                    id={prep.prepId}
                    label={prep.prepName}
                    name="selectedPrepIds"
                    checked={prep.isSelected}
                         actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => renameDialog.show(prep.prepId, prep.prepName, true)
                         }}
            />
        {/each}
    </ul>
</form>