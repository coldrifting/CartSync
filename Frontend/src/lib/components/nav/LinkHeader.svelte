<script lang="ts">
    interface Props {
        url: string;
        title: string;
    }
    
    let {url, title}: Props = $props();
</script>

<style>
    .title-navigation {
        color: var(--bs-primary-text-emphasis);
        fill: none;
        font-size: 1.5rem;
        text-decoration: none;
        
        display: flex;
        flex-direction: row;
        align-items: center;
        
        .label-small {
            display: none;
        }
        
        svg {
            width: 1.2rem;
            height: 1.35rem;
        }
        
        span {
            padding-bottom: 0.125rem;
            padding-left: 0;
        }
        
        &:hover {
            color: var(--bs-primary);
        }
        
        &:active {
            color: var(--bs-primary-border-subtle);
        }
    }
</style>

<a href="{url}" class="title-navigation">
    <svg class="header-icon" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" viewBox="2 0 8 16">
        <path stroke="currentColor" stroke-linecap="round" stroke-width="2"
              d="M8,16 2,8 8,0"/>
    </svg>
    <span class="label text-nowrap">{title}</span>
    <span class="label-small">Back</span>
</a>