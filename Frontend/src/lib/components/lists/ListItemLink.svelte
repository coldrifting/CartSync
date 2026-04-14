<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";
    import ContextInfo from "$lib/components/lists/ContextInfo.svelte";

    interface Props {
        label: string;
        info?: string | undefined;
        subInfo?: string | undefined;
        showArrow?: boolean | undefined;
        actionLeft?: ButtonAction | undefined;
        actionRight?: ButtonAction | undefined;
        href: string;
        isExternalLink?: boolean;
    }

    let {
        label,
        info = undefined,
        subInfo = undefined,
        actionLeft = undefined,
        actionRight = undefined,
        showArrow = undefined,
        href,
        isExternalLink = undefined,
    }: Props = $props()
    
    let infoColor: string = $derived.by(() => {
        if (showArrow) {
            return 'text-muted';
        }
        
        return 'text-info info-offset'
    })
    
    let formatedUrl: string = $derived.by(() => {
        if (!isExternalLink) {
            return href;
        }
        
        if (!href.startsWith('http://') && !href.startsWith('https://')) {
            return 'http://' + href;
        }
        
        return href;
    });
</script>

<style>
    a {
        margin-left: 0.75rem;
        padding-left: 0.75rem;
        margin-right: 0.75rem;
        padding-right: 0.75rem;
        
        line-height: 2.25rem;
        border-radius: 0.375rem;
        
        &:focus-visible {
            outline: 0.25rem solid rgba(var(--bs-info-rgb), 0.5);
        }
    }
</style>

<ListItem actionLeft={actionLeft} actionRight={actionRight}>
    <a href={formatedUrl}
       target={isExternalLink ? "_blank" : ""} 
       class="d-flex flex-row justify-content-between w-100 text-decoration-none text-light align-items-center overflow-hidden text-nowrap" 
       class:me-0={true}
       role={isExternalLink ? "button" : "link"}>
        <div class="d-flex flex-row justify-content-between align-items-center w-100">
            <span class="truncate">{label}</span>
            <ContextInfo info={info} subInfo={subInfo} infoOffset={showArrow}/>
        </div>
        {#if showArrow}
            <svg aria-hidden="true"
                 class="me-2 {infoColor}"
                 xmlns="http://www.w3.org/2000/svg"
                 width="16"
                 height="16"
                 fill="none"
                 viewBox="0 0 16 16">
                <path stroke="currentColor" stroke-linecap="round" stroke-width="1.5" d="M8,0 16,8 8,16"/>
            </svg>
        {/if}
    </a>
</ListItem>