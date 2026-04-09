<script lang="ts">
    import type {PageProps} from './$types';
    import type ItemDetails from "$lib/models/ItemDetails.ts";
    import ModalAdd from "$lib/components/modal/generic/ModalAdd.svelte";
    import ModalRename from "$lib/components/modal/generic/ModalRename.svelte";
    import Header from "$lib/components/nav/Header.svelte";
    import ListItemLink from "$lib/components/lists/ListItemLink.svelte";

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

    const headerActions: HeaderAction[] = [{ label: "Add Item", icon: "fa-plus", action: () => { addDialog.show() }}];
</script>

<svelte:head>
    <title>Ingredients</title>
</svelte:head>

<Header title="Items" headerActions={headerActions} bind:filterText={filterText}/>

<ModalAdd bind:this={addDialog} type="Item"/>
<ModalRename bind:this={renameDialog} type="Item" tryDelete={true}/>

<ul>
    {#each filteredIngredients as ingredient}
        <ListItemLink label={ingredient.name}
                         info={ingredient.location?.aisleName ?? "(Not Set)"}
                         href="/items/{ingredient.id}"
                         actionRight={{
                            label: 'Edit', 
                            icon: 'fa-pencil', 
                            color: 'success', 
                            action: () => renameDialog.show(ingredient.id, ingredient.name, true)
                         }}
        />
    {/each}
</ul>