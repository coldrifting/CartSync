<script lang="ts">
    import {tick} from "svelte";
    import ListItem from "$lib/components/lists/ListItem.svelte";

    interface Props {
        id: string;
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        actionRight?: ButtonAction | undefined;
        name: string;
        checked: boolean;
        isSingle?: boolean | undefined;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        actionRight = undefined,
        name,
        checked,
        isSingle = undefined
    }: Props = $props()

    // Auto submit
    let inputElement: HTMLInputElement;

    const onchange = () => {
        tick().then(() => {
            inputElement.form?.requestSubmit();
        })
    }
</script>

<ListItem actionRight={actionRight}>
    <label class="d-flex flex-row justify-content-start align-items-center w-100 cursor-pointer">
        {#if isSingle}
            <input type='hidden' name='id' value={id} class="cursor-pointer"/>
        {/if}
        <input class="form-check-input p-3"
               type="checkbox"
               name={isSingle ? 'isChecked' : name}
               value={isSingle ? checked : id}
               bind:checked={checked}
               bind:this={inputElement}
               onchange={onchange}/>
        <span class="ms-3 me-auto">{label}</span>
        {#if info !== undefined || subInfo !== undefined}
            <div class="d-flex flex-column align-items-end me-3">
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