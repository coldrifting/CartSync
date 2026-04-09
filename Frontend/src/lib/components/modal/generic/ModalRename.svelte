<script lang="ts">
    import {enhance} from '$app/forms';
    import {trapFocus} from 'trap-focus-svelte'
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import ModalDelete from "$lib/components/modal/generic/ModalDelete.svelte";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";

    interface Props {
        type: string;
        warning?: string | null | undefined;
        tryDelete?: boolean | undefined;
        verb?: string | undefined;
    }

    let {type, warning = undefined, tryDelete = undefined, verb = undefined}: Props = $props();

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

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
        };
    };

    const tryDeleteSubmitFunction: SubmitFunction = () => {
        return async ({update, result}) => {
            if (result.type === 'failure' && result.data) {
                deleteDialog.show(id, name, result.data);
            } else {
                isOpen = false
                await update();
            }
        };
    };

    let deleteForm: HTMLFormElement
    const onDelete = () => {
        if (tryDelete !== undefined) {
            tryDeleteForm.requestSubmit();
        }
        else if (warning !== null) {
            deleteDialog.show(id, name);
        } 
        else {
            deleteForm.requestSubmit();
        }
    }
    
    let tryDeleteForm: HTMLFormElement

    let deleteDialog: ModalDelete
</script>

<ModalDelete bind:this={deleteDialog} type={type} warning={warning ?? ""} bind:parentIsOpen={isOpen}/>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       centered={true}
       on:open={() => document.getElementById("inputRename")?.focus()}>
    <form method="POST"
          action="?/{uiVerb.toLowerCase()}{type.split(' ').join('')}"
          use:enhance={submitFunction}
          use:trapFocus={true}>
        <ModalHeaderCustom title="{uiVerb} {type}" bind:isOpen={isOpen}/>
        <div>
            <input name="id" bind:value={id} required type="hidden"/>
            <FormGroup floating label="{type} Name">
                <Input id="inputRename" name="inputRename" bind:value={value} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            {#if showDeleteButton}
                <Button class="left-button" color="danger" type="button" onclick={onDelete}>Delete</Button>
            {/if}

            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="success" type="submit" disabled={value.trim() === ""}>{uiVerb}</Button>
        </ModalFooter>
    </form>

    <form method="POST"
          action="?/delete{type}"
          bind:this={deleteForm}
          use:enhance={submitFunction}>
        <input name="id" bind:value={id} hidden required/>
    </form>
</Modal>

<form method="POST"
      action="?/tryDelete{type}"
      bind:this={tryDeleteForm}
      use:enhance={tryDeleteSubmitFunction}>
    <input hidden name="id" bind:value={id}/>
    <input hidden type="submit"/>
</form>