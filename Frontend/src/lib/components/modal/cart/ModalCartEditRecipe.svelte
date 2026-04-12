<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {del, put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";

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

    function onfocus(event: Event) {
        let element = event.target as HTMLInputElement;
        element.select();
    }

    function onfocusout(_: Event) {
        recipeQuantity = Math.round(recipeQuantity);
        if (recipeQuantity < 1) {
            recipeQuantity = 1;
        }
    }
    
    function onOpen() {
        document.getElementById('recipeQuantityInput')?.focus();
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
</script>

<ModalCustom title="Edit Cart Entry Recipe"
             bind:isOpen
             action={{label: "Update", action: onSubmit}}
             actionIsDisabled={isSubmitDisabled}
             actionDelete={({label: "Remove", action: onDelete})}
             onOpen={onOpen}>
    <h4>{recipe?.name}</h4>

    <FormGroup floating label="Quantity">
        <Input id="recipeQuantityInput"
               type="number"
               min={0}
               step={1}
               onfocus={onfocus}
               onfocusout={onfocusout}
               bind:value={recipeQuantity}/>
    </FormGroup>
</ModalCustom>