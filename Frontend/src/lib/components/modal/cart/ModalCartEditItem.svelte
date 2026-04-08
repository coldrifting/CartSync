<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import UnitType from "$lib/scripts/classes/UnitType.js";
    import type CartSelectItem from "$lib/scripts/classes/CartSelectItem.ts";
    import Fraction from "$lib/scripts/classes/Fraction.js";

    interface Props {
        formUrl: string;
        cartItems: CartSelectItem[];
    }

    let {formUrl, cartItems}: Props = $props();

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
    let isFractionValid: boolean = $derived(fraction > 0);

    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("fractionInput")?.focus();
            })
        }
    }

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = (itemId: string, prepId: string | null) => {
        item = cartItems.find(i => i.item.id == itemId && i.prep?.id == prepId)
        isOpen = true;
        focus();
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update({reset: false});
        };
    };
    
    const isDisabled = () => {
        return item === undefined || !isFractionValid;
    }
    
    const onfocus = (e: Event) => {
        let element = e.target as HTMLInputElement;
        element.select();
    }
</script>

<Modal body header="Edit Cart Item"
       isOpen={isOpen}
       toggle={toggle}
       centered={true}>
    <form method="POST"
          action="?/{formUrl}"
          id={formUrl}
          use:enhance={submitFunction}>
        <div>
            <input name="itemId" bind:value={itemId} hidden/>
            <input name="prepId" bind:value={prepId} hidden/>
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
        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={isDisabled()}>Update</Button>
        </ModalFooter>
    </form>
</Modal>