<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
    import {trapFocus} from "trap-focus-svelte";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";

    interface Props {
        formUrl: string;
        cartRecipes: CartSelectRecipe[];
    }

    let {formUrl, cartRecipes}: Props = $props();

    let isOpen: boolean = $state(false);

    let recipe: CartSelectRecipe | undefined = $derived(undefined);
    let recipeId: string | undefined = $derived.by(() => {
        return recipe?.id;
    })
    let recipeQuantity: number = $derived.by(() => {
        return recipe?.quantity ?? 1;
    })

    export const show = (recipeId: string) => {
        recipe = cartRecipes.find(r => r.id === recipeId);
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update({reset: false});
        };
    };
    
    const isDisabled = () => {
        return recipe === undefined || recipeQuantity <= 0;
    }
    
    const onfocus = (e: Event) => {
        let element = e.target as HTMLInputElement;
        element.select();
    }
    
    const onfocusout = (_: Event) => {
        recipeQuantity = Math.round(recipeQuantity);
        if (recipeQuantity < 1) {
            recipeQuantity = 1;
        }
    }
    
    let removeForm: HTMLFormElement;
    const onRemove = () => {
        removeForm.requestSubmit();
    }
</script>

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       on:open={() => document.getElementById("recipeQuantityInput")?.focus()}
       centered={true}>
    <form method="POST"
          action="?/{formUrl}"
          id={formUrl}
          use:trapFocus={true}
          use:enhance={submitFunction}>
        <ModalHeaderCustom title="Edit Cart Recipe" bind:isOpen={isOpen} />
        <div>
            <input name="recipeId" bind:value={recipeId} hidden/>
            <h4>{recipe?.name}</h4>

            <FormGroup floating label="Quantity">
                <Input id="recipeQuantityInput"
                       name="recipeQuantity" 
                       type="number"
                       min={0} 
                       step={1} 
                       onfocus={onfocus}
                       onfocusout={onfocusout}
                       bind:value={recipeQuantity}/>
            </FormGroup>
        </div>
        <ModalFooter>
            <Button class="left-button" color="danger" type="button" onclick={onRemove}>Remove</Button>
            
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="primary" type="submit" disabled={isDisabled()}>Update</Button>
        </ModalFooter>
    </form>
    
    <form method="POST"
          action="?/removeCartRecipe"
          bind:this={removeForm}
          use:enhance={submitFunction}>
        <input name="recipeId" bind:value={recipeId} hidden/>
    </form>
</Modal>