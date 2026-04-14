<script lang="ts">
    interface Props {
        label: string;
        icon: string;
        onClick: () => void;
    }

    let {label, icon, onClick, ...rest}: Props = $props();
    
    let isAdd: boolean = $derived(label.toLowerCase().startsWith("add") || label.toLowerCase().startsWith("create"));
    let isEdit: boolean = $derived(label.toLowerCase().startsWith("edit") || label.toLowerCase().startsWith("update"));
    let isHighlighted: boolean = $derived(label.toLowerCase().startsWith("pin") || label.toLowerCase().startsWith("unpin"));
    let isDelete: boolean = $derived(label.toLowerCase().startsWith("remove") || label.toLowerCase().startsWith("delete"));
</script>

<style>
    button {
        background-color: transparent;
        width: 2.5rem;
        display: block;
        padding: 0;
        border-width: 0;
        aspect-ratio: 1 / 1;
        
        --bs-btn-bg: transparent;
        --bs-btn-border-color: transparent;
        --bs-btn-hover-border-color: transparent;
        --bs-btn-active-border-color: transparent;
        
        --bs-btn-color: rgba(var(--bs-light-rgb), 0.6);
        --bs-btn-hover-color: white;
        --bs-btn-active-color: black;
        
        &.btn-primary {
            --bs-btn-hover-bg: var(--bs-primary);
            --bs-btn-hover-border-color: var(--bs-primary-border-subtle);
            --bs-btn-active-bg: white;
            --bs-btn-active-border-color: var(--bs-primary);
            --bs-btn-active-color: var(--bs-primary);
        }
        
        &.btn-success {
            --bs-btn-color: var(--bs-success);
            --bs-btn-hover-bg: var(--bs-success);
            --bs-btn-hover-border-color: var(--bs-success-border-subtle);
            --bs-btn-active-bg: white;
            --bs-btn-active-border-color: var(--bs-success);
            --bs-btn-active-color: var(--bs-success);
        }
        
        &.btn-highlight {
            --bs-btn-color: var(--bs-warning);
            --bs-btn-hover-bg: var(--bs-warning);
            --bs-btn-hover-border-color: var(--bs-warning-border-subtle);
            --bs-btn-active-bg: rgba(var(--bs-warning-rgb), 0.85);
            --bs-btn-active-border-color: var(--bs-warning);
            --bs-btn-hover-color: black;
            --bs-btn-active-color: black;
        }
        
        &.btn-danger {
            --bs-btn-hover-bg: var(--bs-danger);
            --bs-btn-hover-border-color: var(--bs-danger-border-subtle);
            --bs-btn-active-bg: white;
            --bs-btn-active-border-color: var(--bs-danger);
            --bs-btn-active-color: var(--bs-danger);
        }
        
        &:active {
            transform: scale(0.95);
        }
    }
</style>

<button aria-label={label}
        class:btn={true}
        class:rounded-5={true}
        class:btn-primary={isAdd}
        class:btn-success={isEdit}
        class:btn-highlight={isHighlighted}
        class:btn-danger={isDelete}
        {...rest}
        type="button"
        onclick={onClick}>
    <i class="fa fa {icon}"></i>
</button>