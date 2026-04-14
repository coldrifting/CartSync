<script lang="ts">
    interface Props {
        id: string;
        label: string;
        value: string;
        required?: boolean;
        element?: HTMLInputElement | HTMLTextAreaElement;
        type?: string;
        rows?: number;
        autocomplete?: string;
    }
    
    let {id,
        label,
        value = $bindable(),
        required = undefined,
        element = $bindable(undefined),
        type = undefined,
        rows = undefined,
        autocomplete = undefined,
        ...rest
    }: Props = $props();
</script>

<div class="mb-3 form-floating flex-sm-grow-1">
    {#if rows !== undefined && rows > 0}
        <textarea id={id} 
               bind:value
               bind:this={element}
               class:form-control={true} 
               class:text-area={true}
               name={id} 
               required={required !== undefined}
               rows={rows}
               placeholder={label}
               autocomplete={autocomplete}
               {...rest}
        >
        </textarea>
    {:else}
        <input id={id} 
               type={type !== undefined ? type : "text"}
               bind:value
               bind:this={element}
               class:form-control={true} 
               name={id} 
               required={required !== undefined}
               placeholder={label}
               {...rest}
        />
    {/if}
    <label for={id}>
        {label}
    </label>
</div>