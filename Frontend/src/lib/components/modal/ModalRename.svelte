<script lang="ts">
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import {tick} from "svelte";
    
    let id: string = $state('');
    let isOpen: boolean = $state(false);
    let inputRename: string = $state('');
    let {action, header, labelRename}: {
        action: string;
        header: string;
        labelRename: string;
    } = $props();
    
    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("inputRename")?.focus();
            })
        }
    }
    
    const toggle = () => {
        isOpen = !isOpen;
    }
    
    export const show = (inputId: string, inputValue: string | undefined) => {
        id = inputId;
        inputRename = inputValue ?? "";
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
            <input hidden name="id" bind:value={id} required />
            <FormGroup floating label={labelRename}>
                <Input id="inputRename" name="inputRename" bind:value={inputRename} required/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={inputRename.trim() === ""}>Rename</Button>
        </ModalFooter>
    </form>
</Modal>