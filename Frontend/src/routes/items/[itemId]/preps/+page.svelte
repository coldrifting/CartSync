<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {PageProps, SubmitFunction} from './$types';
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import Header from "$lib/components/Header.svelte";
    import ListElementCheckbox from "$lib/components/ListElementCheckbox.svelte";

    let {data}: PageProps = $props();
    let itemId: string = $derived(data.item.id);
    
    let preps = $derived(data.preps);
    
    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    let tryDeleteForm: HTMLFormElement;
    
    let contextActions: ContextAction[] = [
		{ label: "Rename", action: (id: string, value: string | undefined) => {renameDialog.show(id, value)} },
		{ label: "Delete", action: (id: string, value: string | undefined) => {
                deleteId = id;
                deleteName = value ?? "";
                tick().then(() => {
                    tryDeleteForm.requestSubmit();
                })
            } 
        }
    ];
    
    let deleteId = $state('');
    let deleteName = $state('');

    const submitFunction: SubmitFunction = () => {
        return async ({result, update}) => {
            if (result.type === 'failure' && result.data) {
                deleteDialog.show(deleteId, deleteName, result.data);
            } else {
                await update();
            }
        };
    };
    
    const headerActions: HeaderAction[] = [
        {label: "Add Prep", icon: "fa-plus", action: () => {addDialog.show()}}
    ];
</script>

<svelte:head>
    <title>{data.item.name} - Preps</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addPrep" header="Add Prep" labelAdd="Prep Name" />
<ModalRename bind:this={renameDialog} action="renamePrep" header="Rename Prep" labelRename="Prep Name" />
<ModalDelete bind:this={deleteDialog} action="deletePrep" header="Delete Prep" warning="The prep [Name] will be deleted!" />

<form method="POST"
      action="?/tryDeletePrep"
      bind:this={tryDeleteForm}
      use:enhance={submitFunction}>
    <input hidden name="id" bind:value={deleteId}/>
    <input hidden type="submit"/>
</form>

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
            <ListElementCheckbox
                    id={prep.prepId}
                    label={prep.prepName}
                    name="selectedPrepIds"
                    checked={prep.isSelected}
                    contextActions={contextActions}
            />
        {/each}
    </ul>
</form>