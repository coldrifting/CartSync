<script lang="ts">
    interface Props {
        info?: string;
        subInfo?: string;
        infoOffset?: boolean;
    }

    let {
        info = undefined,
        subInfo = undefined,
        infoOffset = undefined,
        ...rest
    }: Props = $props()
</script>

<style>
    span {
        line-height: 1.25rem;
    }
    
    .text-warning {
        line-height: 0.75rem;
        font-size: 0.75rem;
        margin-bottom: 0.25rem;
    }
</style>

<div class="d-flex flex-row justify-content-between" {...rest}>
    {#if info !== undefined || subInfo !== undefined}
        <div class="d-flex flex-column text-end">
            {#if info !== undefined}
                <span class="text-info" class:text-muted={infoOffset || info.startsWith('(')}>{info}</span>
            {/if}
            {#if subInfo !== undefined}
                <span class="text-warning">{subInfo}</span>
            {/if}
        </div>
    {/if}
</div>