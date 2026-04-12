<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import FormLink from "$lib/components/FormLink.svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    
    type T = $$Generic
    interface Props<T> {
        isOpen: boolean;
        itemType: string;
        items: T[];
        getItemName: (item: T) => string;
        selectedItem: T | undefined;
    }

    let {isOpen = $bindable(), itemType, items, getItemName, selectedItem = $bindable()}: Props<T> = $props();

    let filterText: string = $state('');
    let filteredItems: T[] = $derived(items.filter(item => getItemName(item).toLowerCase().includes(filterText.trim().toLowerCase())));
    
    export const show = (updatedItems?: T[] | undefined) => {
        filterText = '';
        if (updatedItems !== undefined) {
            items = updatedItems;
        }
        
        isOpen = true;
    }
    
    const onclick = (selection: T) => {
        selectedItem = selection;
        isOpen = false;
    }
    
    const getArticle = (nextWord: string) => {
        switch (nextWord.toLowerCase()[0]) {
            case 'a':
            case 'e':
            case 'i':
            case 'o':
            case 'u':
                return "an"
            default:
                return "a"
        }
    }
    
    function onOpen() {
        document.getElementById('filterInput')?.focus();
    }
</script>

<ModalCustom title="Select {getArticle(itemType)} {itemType}"
             bind:isOpen
             onOpen={onOpen}>
            <FormGroup floating label="{itemType} Name">
                <Input id="filterInput" bind:value={filterText} />
            </FormGroup>
            <ul>
            {#each filteredItems as item, i}
                <FormLink text={getItemName(item)} onclick={() => {onclick(item)}} showArrow={false} isSubmitButton={i === 0}/>
            {/each}
            </ul>
</ModalCustom>