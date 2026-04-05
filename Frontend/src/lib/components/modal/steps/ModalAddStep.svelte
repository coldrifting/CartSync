<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import type {SubmitFunction} from "@sveltejs/kit";
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";

    interface Props {
        action: string;
    }

    let {action}: Props = $props();

    let isOpen: boolean = $state(false);
    
    let content: string = $state('');

    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("stepContentsInput")?.focus();
            })
        }
    }

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = () => {
        content = '';
        isOpen = true;
        focus();
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

<Modal body header="Add Recipe Step"
       isOpen={isOpen}
       toggle={toggle}
       size="lg"
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={submitFunction}>
        <div>
            <FormGroup floating label="Step Details or Image URL">
                <Input id="stepContentsInput" name="stepContents" type="textarea" class="text-area" rows={5} bind:value={content} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={content.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>