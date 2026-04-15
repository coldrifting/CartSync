<script lang="ts">
    import type {HTMLInputAttributes} from "svelte/elements";

    interface Props  {
        id: string;
        label: string;
        value: any;
        type?: string;
        element?: HTMLInputElement;
    }
    
    let {id, label, value = $bindable(), type = undefined, element = $bindable(undefined), ...rest}: Props & HTMLInputAttributes = $props();
</script>

<div class="mb-3 form-floating flex-sm-grow-1">
    <input id={id} 
           type={type !== undefined ? type : "text"}
           class:form-control={true} 
           bind:this={element}
           bind:value={value}
           name={id} 
           placeholder={label}
           {...rest}
    />
    <label for={id}>
        {label}
    </label>
</div>