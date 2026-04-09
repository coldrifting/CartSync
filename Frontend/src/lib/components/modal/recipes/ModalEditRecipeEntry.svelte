<script lang="ts">
    import {enhance} from '$app/forms';
    import {trapFocus} from 'trap-focus-svelte'
    import {Button, FormGroup, Input, Modal, ModalFooter} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type Prep from "$lib/models/Prep.ts";
    import UnitType from "$lib/models/UnitType.js";
    import Fraction from "$lib/models/Fraction.js";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";
    
    let isOpen: boolean = $state(false);

    let recipeEntryId: string = $state("");
    let itemName: string = $state("");
    let preps: (Prep | null)[] = $state([]);
    let prepId: string | null = $state(null);

    let showPrepsSelect: boolean = $derived.by(() => {
        return preps.length > 0 && (preps.length !== 1 || preps[0] !== null);
    });
    
    let unitType: string = $state(UnitType.Types[0]);
    let fraction: number = $state(1);
    let isFractionValid: boolean = $derived( fraction > 0 );

    export const show = (inputEntryId: string,
                         inputItemName: string,
                         inputPreps: (Prep | null)[],
                         inputPrepId: string | null,
                         inputUnitType: string,
                         inputFraction: Fraction
    ) => {
        recipeEntryId = inputEntryId;
        itemName = inputItemName;
        preps = inputPreps;
        prepId = inputPrepId;
        unitType = inputUnitType;
        fraction = Fraction.asNumber(inputFraction);

        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update();
        };
    };
    
    let deleteForm: HTMLFormElement;
    const onDelete = () => {
        deleteForm.requestSubmit();
    }
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       on:open={() => document.getElementById("fractionInput")?.focus()}
       centered={true}>
    <form method="POST"
          action="?/editRecipeEntry"
          use:trapFocus={true}
          use:enhance={submitFunction}>
        <ModalHeaderCustom title="Update Recipe Entry" bind:isOpen={isOpen} />
        <div>
            <input name="recipeEntryId" bind:value={recipeEntryId} hidden required/>

            <h5>{itemName}</h5>
            
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
                    <Input id="fractionInput" name="fraction" type="number" min={0} step={0.001} bind:value={fraction}/>
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
            <Button color="danger" type="button" class="left-button" onclick={onDelete}>Delete</Button>
            
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="success" type="submit" disabled={!isFractionValid}>Update</Button>
        </ModalFooter>
    </form>
</Modal>

<form method="POST"
      action="?/deleteRecipeEntry"
      bind:this={deleteForm}
      use:enhance={submitFunction}>
    <input hidden name="entryId" bind:value={recipeEntryId}/>
</form>