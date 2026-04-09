<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";

    interface Props {
        id: any;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        group: string;
        selectedValue: any;
        actionRight?: ButtonAction | undefined;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        group,
        selectedValue = $bindable(),
        actionRight = undefined
    }: Props = $props()

    // Auto submit
    let inputElement: HTMLInputElement;

    const onchange = () => {
        inputElement.form?.requestSubmit();
    }
</script>

<ListItem actionRight={actionRight}>
    <label class="d-flex flex-row justify-content-start align-items-center w-100 cursor-pointer">
        <input class="form-check-input p-3"
               type="radio"
               name={group}
               bind:group={selectedValue}
               value={id}
               bind:this={inputElement}
               onchange={onchange}/>
        <span class="ms-3 me-auto">{label}</span>
        {#if info !== undefined || subInfo !== undefined}
            <div class="d-flex flex-column align-items-end">
                {#if info !== undefined}
                    <span class="text-info">{info}</span>
                {/if}
                {#if subInfo !== undefined}
                    <span class="text-warning">{subInfo}</span>
                {/if}
            </div>
        {/if}
    </label>
</ListItem>