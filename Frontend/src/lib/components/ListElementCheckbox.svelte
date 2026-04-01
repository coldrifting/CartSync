<script lang="ts">
    import ContextMenuCustom from "$lib/components/contextMenu/ContextMenuCustom.svelte";
    import {tick} from "svelte";

    interface Props {
        id: string;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        contextActions?: ContextAction[] | undefined;
        name: string;
        checked: boolean;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        contextActions = undefined,
        name,
        checked
    }: Props = $props()

    // Auto submit
    let inputElement: HTMLInputElement;

    const onchange = () => {
        tick().then(() => {
            inputElement.form?.requestSubmit();
        })
    }
</script>

<ContextMenuCustom contextActions={contextActions} id={id} name={label}>
    <label class="list-item d-flex flex-row justify-content-start align-items-center">
        <input class="form-check-input p-3"
               type="checkbox"
               name={name}
               value={id}
               bind:checked={checked}
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