<script lang="ts">
    import type {HTMLTextareaAttributes} from "svelte/elements";

    interface Props {
        id: string;
        label: string;
        value: any;
        type?: string;
        element?: HTMLTextAreaElement;
    }

    let {id, label, value = $bindable(), element = $bindable(undefined), ...rest}: Props & HTMLTextareaAttributes = $props();
</script>

<div class="mb-3 form-floating flex-sm-grow-1">
    <textarea id={id}
              class:form-control={true}
              class:text-area={true}
              bind:value={value}
              bind:this={element}
              name={id}
              placeholder={label}
              {...rest}
    >
    </textarea>
    <label for={id}>
        {label}
    </label>
</div>