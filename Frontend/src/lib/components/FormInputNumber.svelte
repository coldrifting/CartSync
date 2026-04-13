<script lang="ts">
    interface Props {
        id: string;
        label: string;
        value: number;
        required?: boolean;
        element?: HTMLInputElement | HTMLTextAreaElement;
        min?: number;
        step?: number;
    }
    
    let {
        id, 
        label, 
        value = $bindable(),
        required = undefined,
        element = $bindable(undefined),
        min = undefined,
        step = undefined,
        ...rest
    }: Props = $props();
    
    function onfocus(event: Event) {
        let element = event.target as HTMLInputElement;
        element.select();
    }

    function onfocusout(_: Event) {
        value = Math.round(value);
        if (value < 1) {
            value = 1;
        }
    }
</script>

<div class="mb-3 form-floating flex-sm-grow-1">
    <input id={id} 
           type='number'
           bind:value
           bind:this={element}
           class:form-control={true} 
           name={id} 
           required={required !== undefined}
           min={min}
           step={step}
           onfocus={onfocus}
           onfocusout={onfocusout}
           placeholder={label}
           {...rest}
    />
    <label for={id}>
        {label}
    </label>
</div>