<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";
    import ContextInfo from "$lib/components/lists/ContextInfo.svelte";

    interface Props {
        id: any;
        label: string;
        info?: string;
        subInfo?: string;
        group: string;
        selectedValue: any;
        actionRight?: ButtonAction;
        onValueChange?: (value: string) => Promise<void>;
    }

    let {
        id,
        label,
        info = undefined,
        subInfo = undefined,
        group,
        selectedValue = $bindable(),
        actionRight = undefined,
        onValueChange = undefined
    }: Props = $props()

    async function onchange() {
        await onValueChange?.(selectedValue);
    }
</script>

<style>
    input[type="radio"] {
        cursor: pointer;
        margin-left: 0.75rem !important;
    }
</style>

<ListItem actionRight={actionRight}>
    <label class="d-flex flex-row justify-content-start align-items-center w-100 cursor-pointer overflow-hidden text-nowrap">
        <input class="form-check-input p-2p5"
               type="radio"
               name={group}
               bind:group={selectedValue}
               value={id}
               onchange={onchange}/>
        <span class="ms-3 me-auto">{label}</span>
        <ContextInfo info={info} subInfo={subInfo} />
    </label>
</ListItem>