<script lang="ts">
    import {enhance} from '$app/forms';
    import type {SubmitFunction, PageProps} from './$types';
    import {Button, Col, FormGroup, Input, Row} from "@sveltestrap/sveltestrap";
    import ListItem from '$lib/components/ListItem.svelte';
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import {tick} from "svelte";

    let {data}: PageProps = $props();

    let filterTerm = $state('');

    let filter = (items: IngredientByStore[]) => {
        if (!filterTerm) return items;
        let searchText = filterTerm.toLowerCase().trim();
        return items.filter(i => i.itemName.toLowerCase().includes(searchText));
    }

    let filteredIngredients: IngredientByStore[] = $derived(filter(data.ingredients));
    
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
</script>

<svelte:head>
    <title>Ingredients</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addItem" header="Add Item" labelAdd="Item Name" />
<ModalRename bind:this={renameDialog} action="renameItem" header="Rename Item" labelRename="Item Name" />
<ModalDelete bind:this={deleteDialog} action="deleteItem" header="Delete Item" warning="The item [Name] will be deleted!" />

<form method="POST"
      action="?/tryDeleteItem"
      bind:this={tryDeleteForm}
      use:enhance={submitFunction}>
    <input hidden name="id" bind:value={deleteId}/>
    <input hidden type="submit"/>
</form>

<h1>Items</h1>
<Row>
    <Col class="input-group">
        <FormGroup class="mb-3" floating label="Search">
            <Input name="search" id="search" class="rounded-end-2" required bind:value={filterTerm}/>
        </FormGroup>
        <Button class="input-button mb-3 {filterTerm === '' ? 'd-none' : ''}" type="button" onclick={() => {filterTerm = ''}} >
            <i class="fa fa-times"></i>
        </Button>
        <Button color="primary" class="input-side-button mb-3" type="button" onclick={() => {addDialog.show()}}>Add</Button>
    </Col>
</Row>

<div class="list">
    {#each filteredIngredients as ingredient}
        <ListItem name={ingredient.itemName}
                  id={ingredient.itemId}
                  subtitle={ingredient.location?.aisleName}
                  link="/items/{ingredient.itemId}"
                  contextActions={contextActions}
        />
    {/each}
</div>