<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {invalidateAll} from "$app/navigation";

    interface Props {
        type: string;
        addAction: (value: string) => Promise<void>;
        scrollOnAdd?: boolean | undefined;
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
    }
    
    function onOpen() {
        document.getElementById('inputAdd')?.focus();
    }
</script>

<ModalCustom title="Add {type}"
             bind:isOpen
             action={{label: "Add", action: onAdd}}
             onOpen={onOpen}>
        <FormGroup floating label="{type} Name">
            <Input id="inputAdd" name="inputAdd" bind:value={value} required/>
        </FormGroup>
</ModalCustom>