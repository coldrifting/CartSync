<script lang="ts">
    interface ButtonAction {
        label: string;
        action: () => void;
    }
    
    interface Props {
        children: any;
        title: string;
        isOpen: boolean;
        action?: ButtonAction;
        actionIsDisabled?: boolean;
        actionDelete?: ButtonAction;
        autoFocusElement?: HTMLElement;
        isExpanded?: boolean;
    }

    let {
        children, 
        title, 
        isOpen = $bindable(), 
        action, 
        actionIsDisabled = $bindable(undefined), 
        actionDelete = undefined, 
        autoFocusElement = undefined, 
        isExpanded = undefined
    }: Props = $props();
    
    let actionIsUpdate: boolean = $derived.by(() => {
        return action?.label.toLowerCase() === "update" || action?.label.toLowerCase() === "rename";
    });
    
    function hide() {
        isOpen = false;
    }
    
    function onsubmit(event: SubmitEvent) {
        event.preventDefault();
        action?.action();
    }
    
    let dialog: HTMLDialogElement;
    let cancelButton: HTMLButtonElement;
    
    $effect(() => {
        if(isOpen) {
            dialog.showModal();
            if (autoFocusElement !== undefined) {
                autoFocusElement.focus();
            }
            else {
                cancelButton.focus();
            }
        }
        else {
            dialog.close();
        }
    });
    
</script>

<style>
    :global {
        body:has(dialog[open]) {
            overflow: hidden;
        }
        
        dialog {
            ul {
                height: 30dvh;
                overflow-y: auto;
                
                margin-left: -1rem;
                margin-right: -1rem;
                margin-bottom: -1rem;
                
                padding-left: 1rem;
                padding-right: 1rem;
            }
        }
    }
    
    dialog {
        padding: 1rem;
        border: 0;
        margin: auto;
        background: none;
        width: 100%;
        min-width: 320px;
        max-width: 540px;
        
        &.expanded {
            max-width: 840px;
        }
        
        form {
            width: 100%;
        }
        
        .header {
            padding-bottom: 1.5rem;
        }
        
        .body {
            background-color: var(--theme-modal-bg);
            padding: 1rem;
            border-top-left-radius: 0.75rem;
            border-top-right-radius: 0.75rem;
        }
    
        .footer {
            background-color: var(--theme-modal-footer-bg);
            padding: 1rem;
            border-bottom-left-radius: 0.75rem;
            border-bottom-right-radius: 0.75rem;
        }
        
        &::backdrop {
            background-color: rgba(0, 0, 0, 0.4);
        }
    }
</style>

<dialog bind:this={dialog} closedby="any" onclose={() => isOpen = false} class:expanded={isExpanded}>
    <form onsubmit={onsubmit}>
        <div class="body">
            <div class="d-flex flex-row align-items-center justify-content-between header">
                <h5 class="modal-title me-auto">{title}</h5>
                <button type="button" onclick={hide} class="btn-close p-2" aria-label='close'>
                </button>
            </div>
            <div class="content">
                {@render children()}
            </div>
        </div>
        <div class="footer w-full d-flex flex-row align-items-center justify-content-between gap-2">
            {#if actionDelete !== undefined}
                <button class="btn btn-danger me-auto" type="button" onclick={actionDelete.action}>
                    {actionDelete.label}
                </button>
            {/if}

            <button class="btn btn-secondary ms-auto" type="button" onclick={hide} bind:this={cancelButton}>
                Cancel
            </button>
            {#if action}
                <button class="btn" class:btn-success={actionIsUpdate} class:btn-primary={!actionIsUpdate} type="submit" disabled={actionIsDisabled}>
                    {action.label}
                </button>
            {/if}
        </div>
    </form>
</dialog>