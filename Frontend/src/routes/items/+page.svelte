<script lang="ts">
    import {enhance} from '$app/forms';
    import type {SubmitFunction, PageProps} from './$types';
    import ListItem from '$lib/components/ListItem.svelte';
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import {tick} from "svelte";
    import Header from "$lib/components/Header.svelte";

    let {data}: PageProps = $props();
    
    let filterText: string = $state('');
    let filter = (items: IngredientByStore[]) => {
        if (!filterText) return items;
        let searchText = filterText.toLowerCase().trim();
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

<Header title="Items"
        actions={[{label: "Add Item", icon: "fa-plus", action: () => {addDialog.show()}}]}
        bind:filterText={filterText}/>

<ul>
    {#each filteredIngredients as ingredient}
        <ListItem name={ingredient.itemName}
                  id={ingredient.itemId}
                  subtitle={ingredient.location?.aisleName ?? "(Not Set)"}
                  link="/items/{ingredient.itemId}"
                  contextActions={contextActions}
        />
    {/each}
</ul>