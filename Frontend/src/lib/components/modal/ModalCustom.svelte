<script lang="ts">
    import {trapFocus} from 'trap-focus-svelte'
    import {Button, Modal, ModalFooter} from "@sveltestrap/sveltestrap";

    interface ButtonAction {
        label: string;
        action: () => void;
    }
    
    interface Props {
        children: any;
        title: string;
        isOpen: boolean;
        keyboard?: boolean;
        action?: ButtonAction;
        actionIsDisabled?: boolean;
        actionDelete?: ButtonAction;
        onOpen?: () => void;
    }

    let {children, title, isOpen = $bindable(), keyboard, action, actionIsDisabled = $bindable(undefined), actionDelete = undefined, onOpen = undefined}: Props = $props();
    
    let actionIsUpdate: boolean = $derived.by(() => {
        return action?.label.toLowerCase() === "update" || action?.label.toLowerCase() === "rename";
    });
    
    function toggle() {
        isOpen = !isOpen;
    }
    
    function cancel() {
        isOpen = false;
    }
    
    function onsubmit(event: SubmitEvent) {
        event.preventDefault();
        action?.action();
    }
</script>

<Modal body
       isOpen={isOpen}
       toggle={toggle}
       keyboard={keyboard ?? true}
       on:open={() => onOpen?.()}
       centered={true}>
    <form onsubmit={onsubmit} use:trapFocus={true}>
        <div class="d-flex flex-row align-items-center justify-content-between header">
            <h5 class="modal-title me-auto">{title}</h5>
            <button type="button" onclick={cancel} class="btn-close p-2" aria-label='close'>
            </button>
        </div>
        <div class="modal-main">
            {@render children()}
        </div>
        <ModalFooter>
            {#if actionDelete !== undefined}
                <Button color="danger" type="button" onclick={actionDelete.action} class="left-button">
                    {actionDelete.label}
                </Button>
            {/if}

            <Button color="secondary" type="button" onclick={cancel}>
                Cancel
            </Button>
            {#if action}
                <Button color={actionIsUpdate ? 'success' : 'primary' } type="submit" disabled={actionIsDisabled}>
                    {action.label}
                </Button>
            {/if}
        </ModalFooter>
    </form>
</Modal>

