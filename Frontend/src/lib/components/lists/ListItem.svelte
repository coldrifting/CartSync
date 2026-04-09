<script lang="ts">
    import {Button} from "@sveltestrap/sveltestrap";

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

<div class="d-flex flex-row justify-content-between l-item align-items-center">
    {#if actionLeft !== undefined}
        <Button aria-label={actionLeft.label} 
                color={actionLeft.color} 
                type="button" 
                class="rounded-5 ms-2"
                onclick={() => actionLeft.action()}>
            <i class="fa fa-lg {actionLeft.icon}"></i>
        </Button>
    {/if}
    {@render children()}
    {#if actionRight !== undefined}
        <Button aria-label={actionRight.label} 
                color={actionRight.color} 
                type="button" 
                class="rounded-5 m-auto me-2"
                onclick={() => actionRight.action()}>
            <i class="fa fa-lg {actionRight.icon}"></i>
        </Button>
    {/if}
</div>