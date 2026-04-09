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
    
    let content: string = $state('');

    export const show = () => {
        content = '';
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
            tick().then(() => {
                window.scrollTo(0, document.body.scrollHeight);
            });
        };
    };
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
          use:trapFocus={true}
          use:enhance={submitFunction}>
        <ModalHeaderCustom title="Add Recipe Step" bind:isOpen={isOpen}/>
        <div>
            <FormGroup floating label="Step Details or Image URL">
                <Input id="stepContentsInput" name="stepContents" type="textarea" class="text-area" rows={5} bind:value={content} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="primary" type="submit" disabled={content.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>