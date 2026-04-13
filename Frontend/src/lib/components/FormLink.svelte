<script lang="ts">
    interface Props {
        text: string;
        label?: string;
        onclick: () => void; 
        showArrow?: boolean;
        isSubmitButton?: boolean;
        element?: HTMLButtonElement;
    }
    
    let {text, label = undefined, onclick, showArrow = true, isSubmitButton = undefined, element = $bindable(undefined)}: Props = $props();
</script>

<style>
    .form-link {
        background-color: var(--theme-input-bg);
        border: none;
        stroke: var(--theme-fg);
        border-radius: 6px;
        height: 58px;
        width: 100%;
        overflow: hidden;
        margin-bottom: 16px;
        display: flex;
        flex-direction: row;
        align-items: center;
        position: relative;
        
        &:not(:only-child) {
            margin: 0;
            border-radius: 0;
            border-bottom: 1px solid transparent;
        }
    
        &:first-child:not(:only-child) {
            border-top-left-radius: 0.5rem;
            border-top-right-radius: 0.5rem;
        }
        
        &:last-child:not(:only-child) {
            border-bottom-left-radius: 0.5rem;
            border-bottom-right-radius: 0.5rem;
        }
        
        &:only-child {
            border-radius: 0.5rem;
        }
        
        &:hover {
            background-color: var(--bs-primary-border-subtle);
        }
        
        &:active {
            background-color: var(--bs-primary);
        }
        
        .form-link-text {
            position: absolute;
            top: 22px;
            left: 0.75rem;
            
            &.centered {
                position: relative;
                top: 0;
                left: 0.5rem;
            }
        }
        
        .secondary {
            color: rgba(var(--bs-body-color-rgb), .75);
        }
    
        .form-link-label {
            position: absolute;
            top: 4px;
            left: 0.75rem;
            font-size: 0.85rem;
            color: rgba(var(--bs-body-color-rgb), .65);
        }
        
        svg {
            position: absolute;
            right: 1rem;
        }
    }
</style>

<button class="form-link" onclick={onclick} type={isSubmitButton ? 'submit' : 'button'} bind:this={element}>
    {#if label !== undefined}
        <span class="form-link-label">{label}</span>
    {/if}
    <span class="form-link-text"
          class:centered={label === undefined}
          class:secondary={text.startsWith('(')}>
        {text}
    </span>
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