<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";

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

<ListItem actionRight={actionRight}>
    <label class="d-flex flex-row justify-content-start align-items-center w-100 cursor-pointer">
        <input class="form-check-input p-3"
               type="radio"
               name={group}
               bind:group={selectedValue}
               value={id}
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