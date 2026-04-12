<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import ModalDelete from "$lib/components/modal/generic/ModalDelete.svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {invalidateAll} from "$app/navigation";

    interface Props {
        type: string;
        warning?: string | null | undefined;
        verb?: string;
        renameAction: (id: string, value: string) => Promise<void>;
        deleteAction?: (id: string) => Promise<void>;
        tryDeleteAction?: (id: string) => Promise<Record<string, string[]>>;
    }

    let {type, warning = undefined, verb = undefined, renameAction, deleteAction = undefined, tryDeleteAction = undefined}: Props = $props();

    let isOpen: boolean = $state(false);

    let id: string = $state('');
    let name: string = $state('');
    let value: string = $state('');
    let uiVerb: string = $derived(verb ?? "Rename");

    let showDeleteButton: boolean = $state(false);
    
    export const show = (inputId: string, inputValue: string, inputShowDelete: boolean | undefined = undefined) => {
        id = inputId;
        name = inputValue
        value = inputValue;
        showDeleteButton = inputShowDelete ?? false;
        isOpen = true;
    }

    async function onDelete() {
        if (deleteAction === undefined) {
            return;
        }
        
        if (tryDeleteAction !== undefined) {
            const usages = await tryDeleteAction(id);
            if ((usages['Items']?.length ?? 0) === 0 &&
                (usages['Recipes']?.length ?? 0) === 0 &&
                (usages['Preps']?.length ?? 0) === 0) {
                await deleteAction(id);
                isOpen = false;
                await invalidateAll();
            }
            else {
                deleteDialog.show(id, name, usages);
            }
        }
        else if (warning !== null) {
            deleteDialog.show(id, name);
        } 
        else {
            await deleteAction(id);
            isOpen = false;
            await invalidateAll();
        }
    }
    
    let deleteDialog: ModalDelete
    
    async function placeholderDelete(_: string) {
    }

    async function onRename() {
        await renameAction(id, value);
        isOpen = false;
        await invalidateAll();
    }
    
    function onOpen() {
        document.getElementById('inputRename')?.focus();
    }
</script>

<ModalDelete bind:this={deleteDialog} 
             type={type} 
             warning={warning ?? ""} 
             bind:parentIsOpen={isOpen} 
             deleteAction={deleteAction ?? placeholderDelete}/>

<ModalCustom title="{uiVerb} {type}"
             bind:isOpen
             action={{label: uiVerb, action: onRename}}
             actionIsDisabled={value.trim() === ""}
             actionDelete={ showDeleteButton ? ({label: "Remove", action: onDelete}) : undefined }
             onOpen={onOpen}>
        <FormGroup floating label="{type} Name">
            <Input id="inputRename" name="inputRename" bind:value={value} required/>
        </FormGroup>
</ModalCustom>