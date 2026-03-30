<script lang="ts">
    import type {PageProps, SubmitFunction} from './$types';
    import {enhance} from '$app/forms';
    import {Button} from "@sveltestrap/sveltestrap";
    import LinkHeader from "$lib/components/LinkHeader.svelte";
    import ListCheckbox from "$lib/components/ListCheckbox.svelte";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";

    let {data}: PageProps = $props();

    let item: IngredientByStore = $derived(data.item);
    let itemId = $derived(item.itemId);
    let preps = $derived(data.preps);

    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete
    
    let editForm: HTMLFormElement;
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
</script>

<svelte:head>
    <title>{data.item.itemName} - Preps</title>
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

<LinkHeader url="/items/{itemId}" title="Item"/>
<h1 class="text-center">{item.itemName}</h1>
<h4>Preps</h4>

<form method="POST"
      action="?/editPreps"
      bind:this={editForm}
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
    <div class="list">
        {#each preps as prep}
            <ListCheckbox
                    id={prep.prepId}
                    label={prep.prepName}
                    isChecked={prep.isSelected}
                    onchange={() => editForm.requestSubmit()}
                    contextActions={contextActions}
            />
        {/each}
    </div>
</form>

<Button color="primary mt-3 p-2" block onclick={() => {addDialog.show()}}>
    Add Prep
</Button>