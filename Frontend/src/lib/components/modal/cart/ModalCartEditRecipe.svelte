<script lang="ts">
    import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, put} from "$lib/functions/requests.js";
    import FormInputNumber from "$lib/components/FormInputNumber.svelte";
    import {useQueryClient} from "@tanstack/svelte-query";

    interface Props {
        cartRecipes: CartSelectRecipe[];
    }

    let {cartRecipes}: Props = $props();

    const client = useQueryClient();
    
    let isOpen: boolean = $state(false);

    let title = $state("Edit Cart Entry Recipe");
    
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

    let isLoading: boolean = $state(false);
    async function onSubmit() {
        isLoading = true;
        await put(`/api/cart/selection/recipes/${recipeId}/edit`, {quantity: recipeQuantity});
        isLoading = false;
        isOpen = false;
        await client.invalidateQueries({queryKey: ['cart']});
    }

    async function onDelete() {
        title = "Removing Cart Recipe Entry...";
        isLoading = true;
        await del(`/api/cart/selection/recipes/${recipeId}/delete`);
        isLoading = false;
        isOpen = false;
        await client.invalidateQueries({queryKey: ['cart']});
    }
    
    let firstElement: HTMLInputElement | undefined = $state(undefined);
</script>

<ModalCustom bind:title
             bind:isOpen
             bind:isLoading
             action={{label: "Update", action: onSubmit}}
             actionIsDisabled={isSubmitDisabled}
             actionDelete={({label: "Remove", action: onDelete})}
             autoFocusElement={firstElement}>
    <h4>{recipe?.name}</h4>

    <FormInputNumber id="recipeQuantityInput" label="Quantity" min={0} step={1} bind:value={recipeQuantity} bind:element={firstElement} />
</ModalCustom>