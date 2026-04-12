<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";

    interface Props {
        id: string;
        label: string;
        info?: string;
        subInfo?: string;
        actionRight?: ButtonAction;
        name: string;
        checked: boolean;
        isSingle?: boolean;
        onValueChange?: (id: string, value: boolean) => Promise<void>;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        actionRight = undefined,
        name,
        checked,
        isSingle = undefined,
        onValueChange = undefined
    }: Props = $props()

    async function onchange() {
        await onValueChange?.(id, checked);
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