<script lang='ts'>
    import {Button, Col, FormGroup, Input} from "@sveltestrap/sveltestrap";
    import LinkHeader from "$lib/components/nav/LinkHeader.svelte";

    interface Props {
        title: string;
        subtitle?: string | undefined;
        headerActions?: HeaderAction[] | undefined;
        filterText?: string | undefined;
        back?: string[] | undefined;
    }

    let {title, subtitle = undefined, headerActions = undefined, filterText = $bindable(undefined), back}: Props = $props();

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
                        <h1 class="header-title text-nowrap">{title}</h1>
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
            <div class="input-row">
                <Col class="input-group">
                    <FormGroup floating spacing="mb-1" label="Search">
                        <Input name="search" id="search" type="search" class="rounded-end-2" required bind:value={filterText}/>
                    </FormGroup>
                </Col>
            </div>
        {/if}
    </div>
</header>

<div id="sticky-header-padding"
     class="{filterText !== undefined ? 'with-input' : ''} {subtitle !== undefined ? 'with-stacked-title' : ''}">
</div>