<script lang="ts">
    import ContextButton from "$lib/components/lists/ContextButton.svelte";

    interface Props {
        children: any;
        actionLeft?: ButtonAction | undefined;
        actionRight?: ButtonAction | undefined;
    }

    let {
        children,
        actionLeft = undefined,
        actionRight = undefined,
    }: Props = $props()
</script>

<div class="d-flex flex-row justify-content-between list-item align-items-center w-100">
    {#if actionLeft !== undefined}
        <ContextButton label={actionLeft.label} icon={actionLeft.icon} onClick={actionLeft.action} class="ms-3"/>
    {/if}
    {@render children()}
    {#if actionRight !== undefined}
        <ContextButton label={actionRight.label} icon={actionRight.icon} onClick={actionRight.action} class="m-auto me-3"/>
    {/if}
</div>