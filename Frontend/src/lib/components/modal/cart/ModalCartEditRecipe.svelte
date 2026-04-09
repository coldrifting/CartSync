<script lang="ts">
    import {tick} from "svelte";
    import {enhance} from '$app/forms';
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type CartSelectRecipe from "$lib/scripts/classes/CartSelectRecipe.ts";

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

    const focus = () => {
        if (isOpen) {
            tick().then(() => {
                document.getElementById("recipeQuantityInput")?.focus();
            })
        }
    }

    const toggle = () => {
        isOpen = !isOpen;
    }

    export const show = (recipeId: string) => {
        recipe = cartRecipes.find(r => r.id === recipeId);
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
</script>

<Modal body header="Edit Cart Recipe"
       isOpen={isOpen}
       toggle={toggle}
       centered={true}>
    <form method="POST"
          action="?/{formUrl}"
          id={formUrl}
          use:enhance={submitFunction}>
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
            <Button color="secondary" type="button" onclick={toggle}>Cancel</Button>
            <Button color="primary" type="submit" disabled={isDisabled()}>Update</Button>
        </ModalFooter>
    </form>
</Modal>