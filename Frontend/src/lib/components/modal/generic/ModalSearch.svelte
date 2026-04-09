<script lang="ts">
    import {trapFocus} from 'trap-focus-svelte'
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import FormLink from "$lib/components/FormLink.svelte";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";
    
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
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       keyboard={true}
       autoFocus={false}
       on:open={() => document.getElementById("filterInput")?.focus()}
       centered={true}>
    <form use:trapFocus={true}>
        <ModalHeaderCustom title="Select {getArticle(itemType)} {itemType}" bind:isOpen={isOpen}/>
        <div class="modal-main">
            <FormGroup floating label="{itemType} Name">
                <Input id="filterInput" bind:value={filterText} />
            </FormGroup>
            <ul>
            {#each filteredItems as item}
                <FormLink text={getItemName(item)} onclick={() => {onclick(item)}} showArrow={false}/>
            {/each}
            </ul>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
        </ModalFooter>
    </form>
</Modal>