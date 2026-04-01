<script lang="ts">
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";

    interface Props {
        id: any;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        contextActions?: ContextAction[] | undefined;
        group: string;
        selectedValue: any;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        contextActions = undefined,
        group,
        selectedValue = $bindable()
    }: Props = $props()

    // Auto submit
    let inputElement: HTMLInputElement;

    const onchange = () => {
        inputElement.form?.requestSubmit();
    }
</script>

<ContextMenuCustom contextActions={contextActions} id={id} name={label}>
    <label class="list-item d-flex flex-row justify-content-start align-items-center">
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
                    <span class="info">{info}</span>
                {/if}
                {#if subInfo !== undefined}
                    <span class="info-sub">{subInfo}</span>
                {/if}
            </div>
        {/if}
    </label>
</ContextMenuCustom>