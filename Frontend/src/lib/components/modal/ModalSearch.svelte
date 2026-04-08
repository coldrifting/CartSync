<script lang="ts">
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    import FormLink from "$lib/components/FormLink.svelte";
    
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
    
    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("filterInput")?.focus();
            })
        }
    }

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = () => {
        // Reset
        filterText = '';
        isOpen = true;
        focus();
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

<Modal body header="Select {getArticle(itemType)} {itemType}"
       isOpen={isOpen}
       toggle={toggle}
       keyboard={true}
       class="list-select-modal"
       autoFocus={false}
       centered={true}>
    <div>
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
        <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
    </ModalFooter>
</Modal>