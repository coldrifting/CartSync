<script lang="ts">
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    import FormLink from "$lib/components/FormLink.svelte";
    import {ValidItem, type AllValidItems} from "$lib/scripts/classes/ValidItemsAndPreps.js";

    interface Props {
        isOpen: boolean;
        items: AllValidItems;
        selectedItem: ValidItem | undefined;
    }

    let {isOpen = $bindable(), items, selectedItem = $bindable()}: Props = $props();

    let filterText: string = $state('');
    let sectionId: string | undefined = $state(undefined);
    let filteredItems: ValidItem[] = $derived(items.sections
        .filter(section => section.id === sectionId)[0].items
        .filter(item => item.name.toLowerCase().includes(filterText.trim().toLowerCase())));
    
    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("itemFilter")?.focus();
            })
        }
    }

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = (newSectionId: string | undefined) => {
        // Reset
        filterText = '';
        sectionId = newSectionId;
        
        isOpen = true;
        focus();
    }
    
    const onclick = (item: ValidItem) => {
        selectedItem = item;
        isOpen = false;
    }
</script>

<Modal body header="Select an Item"
       isOpen={isOpen}
       toggle={toggle}
       keyboard={true}
       class="list-select-modal"
       centered={true}>
    <div>
        <FormGroup floating label="Item Name">
            <Input id="itemFilter" name="itemFilter" bind:value={filterText} />
        </FormGroup>
        <ul>
        {#each filteredItems as item}
            <FormLink text={item.name} onclick={() => {onclick(item)}} showArrow={false}/>
        {/each}
        </ul>
    </div>
    <ModalFooter>
        <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
    </ModalFooter>
</Modal>