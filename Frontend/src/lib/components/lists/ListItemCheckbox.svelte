<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";
    import ContextInfo from "$lib/components/lists/ContextInfo.svelte";

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

<style>
    input[type="checkbox"] {
        cursor: pointer;
        margin-left: 0.75rem !important;
    }
</style>

<ListItem actionRight={actionRight}>
    <label class="d-flex flex-row justify-content-start align-items-center w-100 cursor-pointer me-3 overflow-hidden text-nowrap">
        {#if isSingle}
            <input type='hidden' name='id' value={id} class="cursor-pointer"/>
        {/if}
        <input class="form-check-input p-2p5"
               type="checkbox"
               name={isSingle ? 'isChecked' : name}
               value={isSingle ? checked : id}
               bind:checked={checked}
               onchange={onchange}/>
        <span class="ms-3 me-auto truncate">{label}</span>
        <ContextInfo info={info} subInfo={subInfo} />
    </label>
</ListItem>