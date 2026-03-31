<script lang='ts'>
    import {Button, Col, FormGroup, Input} from "@sveltestrap/sveltestrap";
    import LinkHeader from "$lib/components/LinkHeader.svelte";

    interface Props {
        title: string;
        subtitle?: string | undefined;
        actions?: HeaderAction[] | undefined;
        filterText?: string | undefined;
        back?: string[] | undefined;
    }

    let {title, subtitle = undefined, actions = undefined, filterText = $bindable(undefined), back}: Props = $props();

    let header: HTMLElement;

    let scrollY: number = $state(0);
    let detached: boolean = $derived(scrollY > 0);

    $effect(() => {
        detached
            ? header.classList.add("detached")
            : header.classList.remove("detached");
    })

</script>

<svelte:window bind:scrollY={scrollY}/>

<header bind:this={header}>
    <div class="header-main">
        <div class="actions-row">
            <div class="start">
                <div>
                    {#if back}
                        <LinkHeader url={back[0]} title={back[1]}/>
                    {:else}
                        <h1>{title}</h1>
                    {/if}
                </div>
            </div>

            <div class="middle">
                <div class="title-container">
                    {#if back}
                        <h4>{title}</h4>
                        {#if subtitle !== undefined}
                            <h6>{subtitle}</h6>
                        {/if}
                    {/if}
                </div>
            </div>

            <div class="end">
                <div>
                    {#each actions as action}
                        <div>
                            <Button color="primary" block type="button" aria-label={action.label}
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
            <div class="input-row">
                <Col class="input-group">
                    <FormGroup floating spacing="mb-1" label="Search">
                        <Input name="search" id="search" class="rounded-end-2" required bind:value={filterText}/>
                    </FormGroup>
                    <Button class="input-button {filterText === '' ? 'd-none' : ''}" type="button"
                            onclick={() => {filterText = ''}}>
                        <i class="fa fa-times"></i>
                    </Button>
                </Col>
            </div>
        {/if}
    </div>
</header>

<div id="sticky-header-padding"
     class="{filterText !== undefined ? 'with-input' : ''} {subtitle !== undefined ? 'with-stacked-title' : ''}">
</div>