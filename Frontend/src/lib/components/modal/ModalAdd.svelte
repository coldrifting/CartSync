<script lang="ts">
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    
    let isOpen: boolean = $state(false);
    let inputAdd: string = $state('');
    let {action, header, labelAdd}: {
        action: string;
        header: string;
        labelAdd: string;
    } = $props();
    
    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("inputAdd")?.focus();
            })
        }
    }
    
    const toggle = () => {
        isOpen = !isOpen;
    }
    
    export const show = () => {
        inputAdd = '';
        isOpen = true;
        focus();
    }
</script>

<Modal body header={header}
       isOpen={isOpen}
       toggle={toggle}
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={() => {isOpen = false}}>
        <div>
            <FormGroup floating label={labelAdd}>
                <Input id="inputAdd" name="inputAdd" bind:value={inputAdd} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={inputAdd.trim() === ""}>Add</Button>
        </ModalFooter>
    </form>
</Modal>