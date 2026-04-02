<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {Button, FormGroup, Input, Modal, ModalFooter} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type Prep from "$lib/scripts/classes/Prep.ts";
    import UnitType from "$lib/scripts/classes/UnitType.js";
    import Fraction from "$lib/scripts/classes/Fraction.js";

    interface Props {
        action: string;
        header: string;
    }

    let {action, header}: Props = $props();

    let isOpen: boolean = $state(false);

    let recipeEntryId: string = $state("");
    let sectionId: string = $state("");
    let itemName: string = $state("");
    let preps: (Prep | null)[] = $state([]);
    let prepId: string | null = $state(null);

    let showPrepsSelect: boolean = $derived.by(() => {
        return preps.length > 0 && (preps.length !== 1 || preps[0] !== null);
    });
    
    let unitType: string = $state(UnitType.Types[0]);
    let fraction: string = $state('1');
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

    export const show = (inputEntryId: string, inputSectionId: string, inputItemName: string, inputPreps: (Prep | null)[], inputPrepId: string | null, inputUnitType: string, inputFraction: Fraction) => {
        recipeEntryId = inputEntryId;
        sectionId = inputSectionId;
        itemName = inputItemName;
        preps = inputPreps;
        prepId = inputPrepId;
        unitType = inputUnitType;
        fraction = Fraction.asString(inputFraction);

        isOpen = true;
        focus();
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
        };
    };

</script>

<Modal body header={header}
       isOpen={isOpen}
       toggle={toggle}
       centered={true}>
    <form method="POST"
          action="?/{action}"
          id={action}
          use:enhance={submitFunction}>
        <div>
            <input name="recipeSectionId" bind:value={sectionId} hidden required/>
            <input name="recipeEntryId" bind:value={recipeEntryId} hidden required/>
            
            <FormGroup floating label="Item" disabled={true}>
                <Input bind:value={itemName}  disabled={true} />
            </FormGroup>

            {#if showPrepsSelect}
                <FormGroup floating label="Prep">
                    <Input type="select"
                           name="prepId"
                           bind:value={prepId}>
                        {#each preps as prep}
                            <option value={prep?.prepId}>{prep?.prepName ?? '(None)'}</option>
                        {/each}
                    </Input>
                </FormGroup>
            {/if}
            <div class="d-flex flex-column flex-sm-row justify-content-between">
                <FormGroup floating label="Amount" class="flex-sm-grow-1">
                    <Input name="fraction" bind:value={fraction}/>
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
            <Button color="primary" type="submit" disabled={!isFractionValid}>Edit</Button>
        </ModalFooter>
    </form>
</Modal>