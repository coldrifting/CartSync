<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import UnitType from "$lib/models/UnitType.js";
    import type CartSelectItem from "$lib/models/CartSelectItem.ts";
    import Fraction from "$lib/models/Fraction.js";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {invalidateAll} from "$app/navigation";
    import Amount from "$lib/models/Amount.js";
    import {put, del} from "$lib/functions/requests.js";

    interface Props {
        cartItems: CartSelectItem[];
    }

    let {cartItems}: Props = $props();

    let isOpen: boolean = $state(false);

    let item: CartSelectItem | undefined = $derived(undefined);
    let itemId: string | undefined = $derived.by(() => {
        return item?.item.id;
    });
    let prepId: string | undefined = $derived.by(() => {
        return item?.prep?.id ?? undefined;
    });

    let unitType: string = $derived.by(() => {
        return item?.amount.unitType ?? UnitType.Types[0];
    });
    let fraction: number = $derived.by(() => {
        return Fraction.asNumber(item?.amount.fraction ?? {num: 1, dem: 1});
    });
    let amount: Amount = $derived.by(() => {
        return {fraction: Fraction.fromNumberString(fraction.toFixed(3)), unitType: unitType} as Amount;
    })
    let isFractionValid: boolean = $derived(fraction > 0);
    
    let isSubmitDisabled: boolean = $derived.by(() => {
        return item === undefined || !isFractionValid;
    })

    export function show(itemId: string, prepId: string | null) {
        item = cartItems.find(i => i.item.id == itemId && i.prep?.id == prepId)
        isOpen = true;
    }

    function onfocus(event: Event) {
        let element = event.target as HTMLInputElement;
        element.select();
    }
    
    function onOpen() {
        document.getElementById('fractionInput')?.focus();
    }

    async function onSubmit() {
        await put(`/api/cart/selection/items/${itemId}/edit` + (prepId !== undefined ? `?prepId=${prepId}` : ''), {amount: amount});
        isOpen = false;
        await invalidateAll();
    }

    async function onDelete() {
        await del(`/api/cart/selection/items/${itemId}/delete` + (prepId !== undefined ? `?prepId=${prepId}` : ''));
        isOpen = false;
        await invalidateAll();
    }
</script>

<ModalCustom title="Edit Cart Entry Item"
             bind:isOpen
             action={({label: "Update", action: onSubmit})}
             actionIsDisabled={isSubmitDisabled}
             actionDelete={{label: "Remove", action: onDelete}}
             onOpen={onOpen}>
            <h4>{item?.item.name}</h4>

            <div class="d-flex flex-column flex-sm-row justify-content-between">
                <FormGroup floating label="Amount" class="flex-sm-grow-1">
                    <Input id="fractionInput" name="fraction" type="number" min={0} step={0.001} bind:value={fraction} onfocus={onfocus}>
                    </Input>
                </FormGroup>
                <FormGroup floating label="Units" class="ms-sm-3">
                    <Input type="select" name="unitType" bind:value={unitType}>
                        {#each UnitType.Types as type}
                            <option value={type}>{UnitType.asString(type)}</option>
                        {/each}
                    </Input>
                </FormGroup>
            </div>
</ModalCustom>