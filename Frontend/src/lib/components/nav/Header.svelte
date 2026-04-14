<script lang='ts'>
    import {Button, Col, FormGroup, Input} from "@sveltestrap/sveltestrap";
    import LinkHeader from "$lib/components/nav/LinkHeader.svelte";
    import FormInputTextSearch from "$lib/components/FormInputTextSearch.svelte";

    interface Props {
        title: string;
        subtitle?: string | undefined;
        headerActions?: HeaderAction[] | undefined;
        filterText?: string | undefined;
        back?: string[] | undefined;
    }

    let {title, subtitle = undefined, headerActions = undefined, filterText = $bindable(undefined), back}: Props = $props();

    let scrollY: number = $state(0);
    let detached: boolean = $derived(scrollY > 0);
</script>

<style>
    header {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        padding: 0.5rem 0 0 0;
        border-bottom: 1px solid transparent;
        z-index: 3;
        
        h1 {
            font-size: 2rem;
            margin: 0;
        }
        
        span {
            padding-right: 0.25rem;
        }
        
        .header-title {
            font-size: 1.6rem;
            
            @media (width >= 576px) {
                font-size: 1.9rem;
            }
        }
        
        .header-main {
            min-width: 320px;
            max-width: 960px;
            padding: 0 1.5rem 0;
            overflow-x: hidden;
            
            @media (width >= 576px) {
                padding: 0 1.5rem 0 calc(var(--sidebar-width) + 1.5rem);
                margin: auto;
            }
        }
    
        &.detached {
            background-color: var(--theme-card-bg);
            border-bottom: 1px solid var(--theme-border);
        }
    
        .actions-row {
            margin-top: 0.25rem;
            margin-bottom: 0.5rem;
        }
        
        .input-row {
            margin-top: 0.75rem;
        }
    }
    
    #sticky-header-padding {
        margin-bottom: 3rem;
        
        &.with-stacked-title {
            margin-bottom: 4.5rem;
        }
        
        &.with-input {
            margin-bottom: 5rem;
        }
    }
</style>

<svelte:window bind:scrollY={scrollY}/>

<header class:detached={detached}>
    <div class="header-main">
        <div class="actions-row d-flex flex-row align-items-center justify-content-between gap-2">
            <div class="start">
                <div>
                    {#if back}
                        <LinkHeader url={back[0]} title={back[1]}/>
                    {:else}
                        <h1 class="header-title truncate">{title}</h1>
                    {/if}
                </div>
            </div>

            <div class="middle flex-fill text-nowrap d-flex flex-column truncate">
                {#if back}
                    <h4 class="text-center truncate">{title}</h4>
                    {#if subtitle !== undefined}
                        <h6 class="text-center truncate">{subtitle}</h6>
                    {/if}
                {/if}
            </div>

            <div class="end">
                <div class="d-flex flex-row gap-2">
                    {#each headerActions as action}
                        <div>
                            <Button color={action.icon.endsWith('refresh') ? "success" : "primary"} block type="button" aria-label={action.label}
                                    onclick={action.action}>
                                <span>{action.label}</span>
                                <i class="fa {action.icon}"></i>
                            </Button>
                        </div>
                    {/each}
                </div>
            </div>
        </div>
        {#if filterText !== undefined}
            <FormInputTextSearch id="search" label="Search" bind:value={filterText}/>
        {/if}
    </div>
</header>

<div id="sticky-header-padding"
     class="{filterText !== undefined ? 'with-input' : ''} {subtitle !== undefined ? 'with-stacked-title' : ''}">
</div>