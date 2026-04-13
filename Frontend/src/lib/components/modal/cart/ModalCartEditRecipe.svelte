<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";
    import FormInputNumber from "$lib/components/FormInputNumber.svelte";

    interface Props {
        cartRecipes: CartSelectRecipe[];
    }

    let {cartRecipes}: Props = $props();

    let isOpen: boolean = $state(false);

    let recipe: CartSelectRecipe | undefined = $derived(undefined);
    let recipeId: string | undefined = $derived.by(() => {
        return recipe?.id;
    })
    let recipeQuantity: number = $derived.by(() => {
        return recipe?.quantity ?? 1;
    })
    let isSubmitDisabled: boolean = $derived.by(() => {
        return recipe === undefined || recipeQuantity <= 0;
    })

    export function show(recipeId: string) {
        recipe = cartRecipes.find(r => r.id === recipeId);
        isOpen = true;
    }

    async function onSubmit() {
        await put(`/api/cart/selection/recipes/${recipeId}/edit`, {quantity: recipeQuantity});
        isOpen = false;
        await invalidateAll();
    }

    async function onDelete() {
        await del(`/api/cart/selection/recipes/${recipeId}/delete`);
        isOpen = false;
        await invalidateAll();
    }
    
    let firstElement: HTMLInputElement | undefined = $state(undefined);
</script>

<ModalCustom title="Edit Cart Entry Recipe"
             bind:isOpen
             action={{label: "Update", action: onSubmit}}
             actionIsDisabled={isSubmitDisabled}
             actionDelete={({label: "Remove", action: onDelete})}
             autoFocusElement={firstElement}>
    <h4>{recipe?.name}</h4>

    <FormInputNumber id="recipeQuantityInput" label="Quantity" min={0} step={1} bind:value={recipeQuantity} bind:element={firstElement} />
</ModalCustom>