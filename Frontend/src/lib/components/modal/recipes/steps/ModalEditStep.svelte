<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {trapFocus} from 'trap-focus-svelte'
    import type {SubmitFunction} from "@sveltejs/kit";
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";

    interface Props {
        action: string;
    }

    let {action}: Props = $props();

    let isOpen: boolean = $state(false);
    
    let stepId: string = $state('');
    let content: string = $state('');

    export const show = (inputStepId: string, inputContent: string) => {
        stepId = inputStepId;
        content = inputContent;
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
        };
    };
    
    let deleteForm: HTMLFormElement
    const onDelete = () => {
        deleteForm.requestSubmit();
    }
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       size="lg"
       on:open={() => document.getElementById("stepContentsInput")?.focus()}
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={submitFunction}
          use:trapFocus={true}>
        <ModalHeaderCustom title="Edit Recipe Step" bind:isOpen={isOpen}/>
        <div>
            <input hidden name="stepId" value={stepId} required/>
            <FormGroup floating label="Step Details or Image URL">
                <Input id="stepContentsInput" name="stepContents" type="textarea" class="text-area" rows={5} bind:value={content} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button class="left-button" color="danger" type="button" onclick={onDelete}>Delete</Button>
            
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="success" type="submit" disabled={content.trim() === ""}>Update</Button>
        </ModalFooter>
    </form>
</Modal>

<form method="POST"
      action="?/deleteStep"
      bind:this={deleteForm}
      use:enhance={submitFunction}>
    <input hidden name="id" bind:value={stepId}/>
</form>