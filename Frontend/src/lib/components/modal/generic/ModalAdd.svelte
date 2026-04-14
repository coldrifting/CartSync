<script lang="ts">
    import {tick} from "svelte";
    import {invalidateAll} from "$app/navigation";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import FormInputText from "$lib/components/FormInputText.svelte";

    interface Props {
        type: string;
        addAction: (value: string) => Promise<void>;
        scrollOnAdd?: boolean;
    }

    let {type, addAction, scrollOnAdd = undefined}: Props = $props();

    let isOpen: boolean = $state(false);
    let value: string = $state('');

    export function show() {
        value = '';
        isOpen = true;
    }
    
    async function onAdd() {
        await addAction(value);
        isOpen = false;
        await invalidateAll();
        
        if (scrollOnAdd) {
            tick().then(() => {
                window.scrollTo(0, document.body.scrollHeight);
            });
        }
    }
    
    let firstElement: HTMLInputElement | undefined = $state(undefined);
</script>

<ModalCustom title="Add {type}"
             bind:isOpen
             action={{label: "Add", action: onAdd}}
             autoFocusElement={firstElement}>
    <FormInputText id="inputAdd" label="{type} Name" bind:element={firstElement} bind:value={value} required/>
</ModalCustom>