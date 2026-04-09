<script lang="ts">
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    import type {SubmitFunction} from "@sveltejs/kit";
    import {trapFocus} from 'trap-focus-svelte'
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";

    interface Props {
        type: string;
        scrollOnAdd?: boolean | undefined;
    }

    let {type, scrollOnAdd = undefined}: Props = $props();

    let isOpen: boolean = $state(false);
    let value: string = $state('');

    export const show = () => {
        value = '';
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
            if (scrollOnAdd === true) {
                tick().then(() => {
                    window.scrollTo(0, document.body.scrollHeight);
                });
            }
        };
    };
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       centered={true}
       on:open={() => document.getElementById("inputAdd")?.focus()}>
    <form method="POST"
          action="?/add{type}"
          use:enhance={submitFunction}
          use:trapFocus={true}>
        <ModalHeaderCustom title="Add {type}" bind:isOpen={isOpen}/>
        <div>
            <FormGroup floating label="{type} Name">
                <Input id="inputAdd" name="inputAdd" bind:value={value} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="primary" type="submit" disabled={value.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>