<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type Prep from "$lib/models/Prep.ts";
    import UnitType from "$lib/models/UnitType.js";
    import Fraction from "$lib/models/Fraction.js";
    import type Amount from "$lib/models/Amount.ts";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, patch} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";
    
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
    let amount: Amount = $derived.by(() => {
        return {fraction: Fraction.fromNumberString(fraction.toFixed(3)), unitType: unitType} as Amount;
    })
    
    let isSubmitDisabled: boolean = $derived(fraction <= 0);
    
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
    
    async function onEdit() {
        await patch(`/api/recipes/entries/${recipeEntryId}/edit`, {'/PrepId': prepId, '/Amount': amount});

        isOpen = false
        await invalidateAll();
    }
    
    async function onDelete() {
        await del(`/api/recipes/entries/${recipeEntryId}/delete`);

        isOpen = false
        await invalidateAll();
    }
    
    function onOpen() {
        document.getElementById('fractionInput')?.focus();
    }
</script>

<ModalCustom title="Update Recipe Entry"
             bind:isOpen
             action={{label: "Update", action: onEdit}}
             actionIsDisabled={isSubmitDisabled}
             actionDelete={{label: "Delete", action: onDelete}}
             onOpen={onOpen}>
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
</ModalCustom>