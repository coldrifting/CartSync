<script lang="ts">
    import ModalDelete from "$lib/components/modal/generic/ModalDelete.svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import FormInputText from "$lib/components/FormInputText.svelte";

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
    let title: string = $derived(uiVerb + ' ' + type);

    let showDeleteButton: boolean = $state(false);
    
    export const show = (inputId: string, inputValue: string, inputShowDelete: boolean | undefined = undefined) => {
        id = inputId;
        name = inputValue
        value = inputValue;
        showDeleteButton = inputShowDelete ?? false;
        isOpen = true;
    }

    let isLoading: boolean = $state(false);
    async function onDelete() {
        if (deleteAction === undefined) {
            return;
        }
        
        title = `Deleting ${type}...`
        if (tryDeleteAction !== undefined) {
            isLoading = true;
            const usages = await tryDeleteAction(id);
            if ((usages['Items']?.length ?? 0) === 0 &&
                (usages['Recipes']?.length ?? 0) === 0 &&
                (usages['Preps']?.length ?? 0) === 0) {
                await deleteAction(id);
                isLoading = false;
                isOpen = false;
            }
            else {
                isLoading = false;
                title = uiVerb + ' ' + type;
                deleteDialog.show(id, name, usages);
                isOpen = false;
            }
        }
        else if (warning !== null) {
            deleteDialog.show(id, name);
            isOpen = false;
        } 
        else {
            isLoading = true;
            await deleteAction(id);
            isLoading = false;
            isOpen = false;
        }
    }
    
    let deleteDialog: ModalDelete
    
    async function placeholderDelete(_: string) {
    }

    async function onRename() {
        isLoading = true;
        await renameAction(id, value);
        isLoading = false;
        isOpen = false;
    }
    
    let firstElement: HTMLInputElement | undefined = $state(undefined);
</script>

<ModalDelete bind:this={deleteDialog} 
             type={type} 
             warning={warning ?? ""} 
             bind:parentIsOpen={isOpen} 
             deleteAction={deleteAction ?? placeholderDelete}/>

<ModalCustom bind:title="{title}"
             bind:isOpen
             bind:isLoading
             action={{label: uiVerb, action: onRename}}
             actionIsDisabled={value.trim() === ""}
             actionDelete={ showDeleteButton ? ({label: "Delete", action: onDelete}) : undefined }
             autoFocusElement={firstElement}>
    <FormInputText id="inputRename" label="{type} Name" bind:element={firstElement} bind:value={value} required/>
</ModalCustom>