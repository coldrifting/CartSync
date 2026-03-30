<script lang="ts">
    import {enhance} from '$app/forms';
    import type {PageProps} from './$types';
    import ListItem from '$lib/components/ListItem.svelte';
    import {
        Button,
        Col,
        FormGroup,
        Input,
        Modal,
        ModalFooter,
        Row
    } from "@sveltestrap/sveltestrap";

    let {data}: PageProps = $props();

    let filterTerm = $state('');
    let allItemNames = $derived(data.ingredients.map(s => s.itemName.toLowerCase()));

    let filter = (items: IngredientByStore[]) => {
        if (!filterTerm) return items;
        let searchText = filterTerm.toLowerCase().trim();
        return items.filter(i => i.itemName.toLowerCase().includes(searchText));
    }

    let filteredIngredients: IngredientByStore[] = $derived(filter(data.ingredients));

    const filterValidForNewItem = () => {
        let notEmpty = filterTerm.trim().length !== 0;
        let notAlreadyExists = !allItemNames.includes(filterTerm.toLowerCase().trim());

        return notEmpty && notAlreadyExists;
    }

    let deleteId = $state('');
    let renameId = $state('');
    let newName = $state('');
    let itemInUseText = $state('');

    $effect(() => {
        if (deleteId.length != 0) {
            let tryDeleteForm = document.getElementById("tryDeleteForm") as HTMLFormElement;
            if (tryDeleteForm) {
                tryDeleteForm.requestSubmit();
            }
        }
    });

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
</script>

<svelte:head>
    <title>Ingredients</title>
</svelte:head>

<Modal body header="Rename Item"
       isOpen={showRenameDialog}
       toggle={toggleRenameDialog}
       centered={true}>
    <form method="POST"
          action="?/rename"
          id="renameForm"
          use:enhance={() => {closeRenameDialog()}}>
        <div>
            <input hidden name="id" bind:value={renameId}/>
            <FormGroup floating label="Item Name">
                <Input name="newName" bind:value={newName} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={closeRenameDialog}>Cancel</Button>
            <Button color="primary" type="submit" disabled={newName.trim() === ""}>Rename</Button>
        </ModalFooter>
    </form>
</Modal>


<Modal body header="Delete Item"
       isOpen={showDeleteDialog}
       toggle={toggleDeleteDialog}
       centered={true}>
    <form method="POST"
          action="?/delete"
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
      action="?/tryDelete"
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

<h1>Items</h1>
<form method="POST"
      action="?/addIngredient"
      use:enhance={() => {
          return async ({ update }) => {
            await update({ reset: false });
        };
      }}>
    <input name="itemName" value="{filterTerm}" hidden/>
    <Row>
        <Col class="input-group">
            <FormGroup class="mb-3" floating label="Search">
                <Input name="search" id="search" class="rounded-end-2" required bind:value={filterTerm}/>
            </FormGroup>
            <Button class="input-button mb-3 {filterTerm === '' ? 'd-none' : ''}" type="button" onclick={() => {filterTerm = ''}} >
                <i class="fa fa-times"></i>
            </Button>
            {#if filterValidForNewItem()}
                <Button color="primary" class="input-side-button mb-3" type="submit">Add</Button>
            {/if}
        </Col>
    </Row>

    <div>
        <ul>
            {#each filteredIngredients as ingredient, i}
                <ListItem name={ingredient.itemName}
                          id={ingredient.itemId}
                          subtitle={ingredient.location?.aisleName ?? "(No Location)"}
                          isSubtitleActive={ingredient.location?.aisleName !== undefined}
                          link="/items/{ingredient.itemId}"
                          actions={actions}
                />
            {/each}
        </ul>
    </div>
</form>
