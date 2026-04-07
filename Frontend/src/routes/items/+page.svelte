<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {SubmitFunction, PageProps} from './$types';
    import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";
    import ModalAdd from "$lib/components/modal/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/ModalRename.svelte";
    import ModalDelete from "$lib/components/modal/ModalDelete.svelte";
    import Header from "$lib/components/Header.svelte";
    import ListElementLink from "$lib/components/ListElementLink.svelte";

    let {data}: PageProps = $props();

    let filterText: string = $state('');
    let filter = (items: ItemDetails[]) => {
        if (!filterText) return items;
        let searchText = filterText.toLowerCase().trim();
        return items.filter(item => item.name.toLowerCase().includes(searchText));
    }

    let filteredIngredients: ItemDetails[] = $derived(filter(data.ingredients));

    let addDialog: ModalAdd
    let renameDialog: ModalRename
    let deleteDialog: ModalDelete

    let tryDeleteForm: HTMLFormElement;

    let contextActions: ContextAction[] = [
        {
            label: "Rename", action: (id: string, value: string | undefined) => {
                renameDialog.show(id, value)
            }
        },
        {
            label: "Delete", action: (id: string, value: string | undefined) => {
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
        {
            label: "Add Item", icon: "fa-plus", action: () => {
                addDialog.show()
            }
        }
    ];
</script>

<svelte:head>
    <title>Ingredients</title>
</svelte:head>

<ModalAdd bind:this={addDialog} action="addItem" header="Add Item" labelAdd="Item Name"/>
<ModalRename bind:this={renameDialog} action="renameItem" header="Rename Item" labelRename="Item Name"/>
<ModalDelete bind:this={deleteDialog} action="deleteItem" header="Delete Item"
             warning="The item [Name] will be deleted!"/>

<form method="POST"
      action="?/tryDeleteItem"
      bind:this={tryDeleteForm}
      use:enhance={submitFunction}>
    <input hidden name="id" bind:value={deleteId}/>
    <input hidden type="submit"/>
</form>

<Header title="Items"
        headerActions={headerActions}
        bind:filterText={filterText}/>

<ul>
    {#each filteredIngredients as ingredient}
        <ListElementLink id={ingredient.id}
                         label={ingredient.name}
                         info={ingredient.location?.aisleName ?? "(Not Set)"}
                         link="/items/{ingredient.id}"
                         contextActions={contextActions}
        />
    {/each}
</ul>