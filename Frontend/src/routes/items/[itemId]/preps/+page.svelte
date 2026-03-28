<script lang="ts">
    import type {PageProps} from './$types';
    import LinkHeader from "$lib/components/LinkHeader.svelte";
    import {Button, FormGroup, Input, Modal, ModalFooter} from "@sveltestrap/sveltestrap";
    import {enhance} from '$app/forms';
    import ListCheckbox from "$lib/components/ListCheckbox.svelte";

    let {data}: PageProps = $props();
    
    let item: IngredientByStore = $derived(data.item);
    let itemId = $derived(item.itemId);
    let preps = $derived(data.preps);
    
    let prepForm: HTMLFormElement;
    
    let renameId = $state('');
    let deleteId = $state('');
    let newName = $state('');
    let itemInUseText = $state('');
    
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
            }
        }
    ];

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
    
    $effect(() => {
        if (deleteId.length != 0) {
            let tryDeleteForm = document.getElementById("tryDeleteForm") as HTMLFormElement;
            if (tryDeleteForm) {
                tryDeleteForm.requestSubmit();
            }
        }
    });
</script>

<svelte:head>
    <title>{data.item.itemName} - Preps</title>
</svelte:head>

<Modal body header="Add Prep"
       isOpen={showAddDialog}
       toggle={toggleAddDialog}
       centered={true}>
    <form method="POST"
          action="?/addPrep"
          id="addForm"
          use:enhance={() => {closeAddDialog()}}>
        <div>
            <FormGroup floating label="Prep Name">
                <Input name="prepName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeAddDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Rename Prep"
       isOpen={showRenameDialog}
       toggle={toggleRenameDialog}
       centered={true}>
    <form method="POST"
          action="?/renamePrep"
          id="renameForm"
          use:enhance={() => {closeRenameDialog()}}>
        <div>
            <input hidden name="id" bind:value={renameId}/>
            <FormGroup floating label="Prep Name">
                <Input name="prepName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeRenameDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""} >Rename</Button>
        </ModalFooter>
    </form>
</Modal>

<Modal body header="Delete Prep"
       isOpen={showDeleteDialog}
       toggle={toggleDeleteDialog}
       centered={true}>
    <form method="POST"
          action="?/deletePrep"
          id="deleteForm"
          use:enhance={() => {closeDeleteDialog()}}>
        <div>
            <input hidden name="id" bind:value={deleteId}/>
            <p class="whitespace-pre-line">{itemInUseText}</p>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeDeleteDialog}>Cancel</Button>
            <Button color="danger" type="submit">Delete</Button>
        </ModalFooter>
    </form>
</Modal>

<form method="POST"
      action="?/tryDeletePrep"
      id="tryDeleteForm"
      use:enhance={() => {
            return async ({ result, update }) => {
                if (result.type === 'failure' && result.data) {
                    itemInUseText = (result.data ?? "").toString()
                    showDeleteDialog = true;
                } else {
                    await update();
                }
            };
        }}>
    <input hidden name="id" bind:value={deleteId}/>
    <input hidden type="submit"/>
</form>

<LinkHeader url="/items/{itemId}" title="Item"/>
<h1>Preps</h1>

<form method="POST"
      action="?/editPreps"
      bind:this={prepForm}
      use:enhance>
    <input name="itemId" bind:value={itemId} hidden/>
<ul>
{#each preps as prep}
    <ListCheckbox 
            id={prep.prepId} 
            label={prep.prepName} 
            isChecked={prep.isSelected} 
            onchange={() => prepForm.requestSubmit()}
            actions={actions}
    />
{/each}
</ul>
</form>

<Button color="primary" onclick={() => {
                newName = "";
                showAddDialog = true;
}}>
    Add Prep
</Button>