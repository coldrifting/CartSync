<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type RecipeSection from "$lib/scripts/classes/RecipeSection.ts";
    import type Prep from "$lib/scripts/classes/Prep.ts";
    import UnitType from "$lib/scripts/classes/UnitType.js";
    import Fraction from "$lib/scripts/classes/Fraction.js";
    import ModalSearchIngredient from "$lib/components/modal/ingredients/ModalSearchIngredient.svelte";
    import FormLink from "$lib/components/FormLink.svelte";
    import {ValidItem, type AllValidItems} from "$lib/scripts/classes/ValidItemsAndPreps.js";

    interface Props {
        action: string;
        header: string;
        sections: RecipeSection[];
        items: AllValidItems;
        scrollOnAdd?: boolean;
    }

    let {action, header, sections, items, scrollOnAdd = undefined}: Props = $props();

    let isOpen: boolean = $state(false);

    let sectionId: string | undefined = $derived(sections.length > 0 ? sections[0].id : undefined);
    let newSectionName: string = $derived.by(() => {
        sectionId;
        return "New Section";
    })

    let item: ValidItem | undefined = $derived.by(() => {
        sectionId; // Reset when section changes
        return undefined;
    });
    let itemId: string | undefined = $derived.by(() => {
        return item?.id;
    });

    let preps: (Prep | null)[] = $derived.by(() => {
        return item?.preps ?? []
    });
    let prepId: string | undefined = $derived.by(() => {
        return preps[0]?.id ?? undefined;
    });
    let showPrepsSelect: boolean = $derived.by(() => {
        return preps.length > 0 && (preps.length !== 1 || preps[0] !== null);
    });

    let unitType: string = $derived.by(() => {
        // Reset when item or section changes
        sectionId;
        return item?.defaultUnitType ?? UnitType.Types[0];
    });

    let fraction: string = $derived.by(() => {
        // Reset when item or section changes
        sectionId;
        item;
        return '1';
    });
    let isFractionValid: boolean = $derived(Fraction.isValid(fraction));

    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("sectionIdSelect")?.focus();
            })
        }
    }
    
    $effect(() => {
        if (sectionId === undefined) {
            tick().then(() => {
                document.getElementById("sectionNameEdit")?.focus();
            })
        }
    })

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = () => {
        reset();

        isOpen = true;
        focus();
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
            if (scrollOnAdd === true) {
                tick().then(() => {
                    window.scrollTo(0, document.body.scrollHeight);
                });
            }
        };
    };

    const reset = () => {
        sectionId = sections.length > 0 ? sections[0].id : undefined;
        item = undefined;
        prepId = undefined;
        fraction = '1';
        unitType = UnitType.Types[0];
    }

    let isItemSearchOpen: boolean = $state(false);
    let allowEscapeKey: boolean = $derived(!isItemSearchOpen);
    let modalSearchIngredient: ModalSearchIngredient;
</script>

<ModalSearchIngredient bind:this={modalSearchIngredient} bind:isOpen={isItemSearchOpen} items={items} bind:selectedItem={item}/>

<Modal body header={header}
       isOpen={isOpen}
       toggle={toggle}
       keyboard={allowEscapeKey}
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={submitFunction}>
        <div>
            <FormGroup floating label="Recipe Section">
                <Input type="select"
                       name="recipeSectionId"
                       id="sectionIdSelect"
                       bind:value={sectionId}>
                    {#each sections as section}
                        <option value={section.id}>{section.name}</option>
                    {/each}
                    <option value={undefined}>(New Section)</option>
                </Input>
            </FormGroup>

            {#if sectionId === undefined}
                <FormGroup floating label="New Recipe Section Name">
                    <Input name="recipeSectionName" id="sectionNameEdit" bind:value={newSectionName}/>
                </FormGroup>
            {/if}

            <input name="itemId" bind:value={itemId} hidden required/>
            <FormLink text={item?.name ?? "(No Item Selected)"} label="Ingredient Selection"
                      onclick={() => modalSearchIngredient.show(sectionId)}/>

            {#if showPrepsSelect}
                <FormGroup floating label="Prep">
                    <Input type="select"
                           name="prepId"
                           bind:value={prepId}>
                        {#each preps as prep}
                            <option value={prep?.id}>{prep?.name ?? '(None)'}</option>
                        {/each}
                    </Input>
                </FormGroup>
            {/if}
            <div class="d-flex flex-column flex-sm-row justify-content-between">
                <FormGroup floating label="Amount" class="flex-sm-grow-1">
                    <Input name="fraction"
                           bind:value={fraction}>
                    </Input>
                </FormGroup>
                <FormGroup floating label="Units" class="ms-sm-3">
                    <Input type="select"
                           name="unitType"
                           bind:value={unitType}>
                        {#each UnitType.Types as type}
                            <option value={type}>{UnitType.asString(type)}</option>
                        {/each}
                    </Input>
                </FormGroup>
            </div>
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={item === undefined || !isFractionValid}>Add</Button>
        </ModalFooter>
    </form>
</Modal>