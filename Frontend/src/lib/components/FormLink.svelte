<script lang="ts">
    interface Props {
        text: string;
        label?: string | undefined;
        onclick: () => void; 
        showArrow?: boolean | undefined;
    }
    
    let {text, label = undefined, onclick, showArrow = true}: Props = $props();
</script>

<button class="form-link" onclick={onclick} type="button">
    {#if label !== undefined}
        <span class="form-link-label">{label}</span>
    {/if}
    <span class="form-link-text {text.startsWith('(') ? 'secondary' : ''} {label === undefined ? 'centered' : ''}">{text}</span>
    {#if showArrow}
        <svg aria-hidden="true"
             xmlns="http://www.w3.org/2000/svg"
             width="16"
             height="16"
             fill="none"
             viewBox="0 0 16 16">
            <path stroke="strokeColor" stroke-linecap="round" stroke-width="1.5"
                  d="M8,0 16,8 8,16"/>
        </svg>
    {/if}
</button>