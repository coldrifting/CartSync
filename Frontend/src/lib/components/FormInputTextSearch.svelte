<script lang="ts">
    interface Props {
        id: string;
        label: string;
        value: string;
    }
    
    let {id, label, value = $bindable(), ...rest}: Props = $props();
</script>

<style>
    input[type="search"]::-webkit-search-cancel-button {
        background-color: rgb(var(--theme-fg-rgb), 0.5);
        -webkit-appearance: none;
        cursor: pointer;
    
        height: 0.9rem;
        width: 0.9rem;
        margin-bottom: 0.8rem;
        margin-right: 0.4rem;
        mask-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23000'%3e%3cpath d='M.293.293a1 1 0 0 1 1.414 0L8 6.586 14.293.293a1 1 0 1 1 1.414 1.414L9.414 8l6.293 6.293a1 1 0 0 1-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 0 1-1.414-1.414L6.586 8 .293 1.707a1 1 0 0 1 0-1.414z'/%3e%3c/svg%3e");
        -webkit-mask-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16' fill='%23000'%3e%3cpath d='M.293.293a1 1 0 0 1 1.414 0L8 6.586 14.293.293a1 1 0 1 1 1.414 1.414L9.414 8l6.293 6.293a1 1 0 0 1-1.414 1.414L8 9.414l-6.293 6.293a1 1 0 0 1-1.414-1.414L6.586 8 .293 1.707a1 1 0 0 1 0-1.414z'/%3e%3c/svg%3e");
    }
    
    input[type="search"]::-webkit-search-cancel-button:hover {
            background-color: var(--bs-danger-text-emphasis);
    }
    
    input[type="search"]::-webkit-search-cancel-button:active {
        background-color: var(--bs-danger);
    }
    
    .form-floating > .form-control {
        height: calc(2.75rem + 2px) !important;
        min-height: calc(2.75rem + 2px) !important;
        line-height: 2.75rem !important;
    }
</style>

<div class="mb-2 form-floating flex-sm-grow-1">
    <input id={id} 
           type='search'
           bind:value
           class:form-control={true} 
           name={id}
           placeholder={label}
           {...rest}
    />
    <label for={id}>
        {label}
    </label>
</div>