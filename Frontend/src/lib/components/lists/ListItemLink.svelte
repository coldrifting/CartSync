<script lang="ts">
    import ListItem from "$lib/components/lists/ListItem.svelte";

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
        console.log(href);
        if (!isExternalLink) {
            return href;
        }
        
        if (!href.startsWith('http://') && !href.startsWith('https://')) {
            return 'http://' + href;
        }
        
        return href;
    });
</script>

<ListItem actionLeft={actionLeft} actionRight={actionRight}>
    <a href={formatedUrl}
       target={isExternalLink ? "_blank" : ""} 
       class="d-flex flex-row justify-content-between w-100 text-decoration-none text-light align-items-center" role={isExternalLink ? "button" : "link"}>
        <div class="d-flex flex-row justify-content-between align-items-center w-100">
            <span class="w-full">{label}</span>
            <div class="d-flex flex-row justify-content-between align-items-center">
                {#if info !== undefined || subInfo !== undefined}
                    <div class="d-flex flex-column">
                        {#if info !== undefined}
                            <span class="{infoColor}">{info}</span>
                        {/if}
                        {#if subInfo !== undefined}
                            <span class="text-warning">{subInfo}</span>
                        {/if}
                    </div>
                {/if}
            </div>
        </div>
        {#if showArrow}
            <svg aria-hidden="true"
                 class={infoColor}
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