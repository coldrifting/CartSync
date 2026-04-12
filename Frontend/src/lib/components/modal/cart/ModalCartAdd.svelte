<script lang="ts">
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type Prep from "$lib/models/Prep.ts";
    import UnitType from "$lib/models/UnitType.js";
    import type Recipe from "$lib/models/Recipe.ts";
    import FormLink from "$lib/components/FormLink.svelte";
    import ItemWithPreps from "$lib/models/ItemWithPreps.js";
    import ModalSearch from "$lib/components/modal/generic/ModalSearch.svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import Fraction from "$lib/models/Fraction.js";
    import type Amount from "$lib/models/Amount.ts";
    import {put} from "$lib/functions/requests.js";
    import {invalidateAll} from "$app/navigation";

    interface Props {
        remainingRecipes: Recipe[];
        remainingItems: ItemWithPreps[];
    }

    let {remainingRecipes, remainingItems}: Props = $props();

    let isOpen: boolean = $state(false);
    let isRecipeSelectionEnabled: boolean = $state(true);

    let recipe: Recipe | undefined = $derived.by(() => {
        // Reset when switching between types
        isRecipeSelectionEnabled;
        return undefined;
    });
    let recipeId: string | undefined = $derived.by(() => {
        return recipe?.id;
    })
    let recipeQuantity: number = $derived.by(() => {
        // Reset when recipe changes
        recipe;
        return 1;
    })

    let item: ItemWithPreps | undefined = $derived.by(() => {
        // Reset when switching between types
        isRecipeSelectionEnabled;
        return undefined;
    });
    let itemId: string | undefined = $derived.by(() => {
        return item?.item.id;
    });

    let validItem: ItemWithPreps | undefined = $derived.by(() => {
        return remainingItems.find(i => i.item.id == itemId);
    });
    let preps: (Prep | null)[] = $derived.by(() => {
        return validItem?.preps ?? []
    });
    let prepId: string | undefined = $derived.by(() => {
        return preps[0]?.id ?? undefined;
    });
    let showPrepsSelect: boolean = $derived.by(() => {
        return validItem?.hasExtraPreps ?? false;
    });

    let unitType: string = $derived.by(() => {
        return validItem?.item.defaultUnitType ?? UnitType.Types[0];
    });
    let fraction: number = $derived.by(() => {
        // Reset when item changes
        item;
        return 1;
    });
    let isFractionValid: boolean = $derived(fraction > 0);
    let amount: Amount = $derived.by(() => {
        return {fraction: Fraction.fromNumberString(fraction.toFixed(3)), unitType: unitType} as Amount;
    })
    
    let isSubmitDisabled: boolean = $derived.by(() => {
        if (isRecipeSelectionEnabled) {
            return recipe === undefined || recipeQuantity <= 0;
        }
        else {
            return item === undefined || !isFractionValid;
        }
    })

    export function show() {
        recipe = undefined;
        item = undefined;
        prepId = undefined;
        isOpen = true;
    }

    const getRecipeName = (recipe: Recipe) => {
        return recipe.name;
    }
    const getItemName = (item: ItemWithPreps) => {
        return item.item.name;
    }
    
    const onfocus = (e: Event) => {
        let element = e.target as HTMLInputElement;
        element.select();
    }
    
    const onRecipeClick = () => {
        // Preselect next element for when modal closes
        document.getElementById("recipeQuantityInput")?.focus();
        modalSearchRecipe.show();
    }
    
    const onItemClick = () => {
        // Preselect next element for when modal closes
        document.getElementById("fractionInput")?.focus();
        modalSearchItem.show();
    }
    
    const onfocusout = (_: Event) => {
        recipeQuantity = Math.round(recipeQuantity);
        if (recipeQuantity < 1) {
            recipeQuantity = 1;
        }
    }

    let isRecipeSearchOpen: boolean = $state(false);
    let isItemSearchOpen: boolean = $state(false);
    let allowEscapeKey: boolean = $derived(!isRecipeSearchOpen && !isItemSearchOpen);

    let modalSearchRecipe: ModalSearch<Recipe>;
    let modalSearchItem: ModalSearch<ItemWithPreps>;
    
    async function onSubmit() {
        if (itemId !== undefined) {
            await put(`/api/cart/selection/items/${itemId}/edit` + (prepId !== undefined ? `?prepId=${prepId}` : ''), {amount: amount});
        }
        else {
            await put(`/api/cart/selection/recipes/${recipeId}/edit`, {quantity: recipeQuantity});
        }
        
        isOpen = false;
        await invalidateAll();
    }
</script>

<ModalSearch bind:this={modalSearchRecipe} 
             bind:isOpen={isRecipeSearchOpen} 
             itemType="Recipe" 
             items={remainingRecipes}
             getItemName={getRecipeName} 
             bind:selectedItem={recipe}/>

<ModalSearch bind:this={modalSearchItem} 
             bind:isOpen={isItemSearchOpen} 
             itemType="Item" 
             items={remainingItems}
             getItemName={getItemName} 
             bind:selectedItem={item}/>

<ModalCustom title="Add Cart Entry"
             bind:isOpen
             keyboard={allowEscapeKey}
             action={{label: "Add", action: onSubmit}}
             actionIsDisabled={isSubmitDisabled}>
            <FormGroup floating label="Cart Item Type">
                <Input id="isRecipeTypeInput" name="isRecipeType" type="select" bind:value={isRecipeSelectionEnabled}>
                    <option value={true}>Recipe</option>
                    <option value={false}>Item</option>
                </Input>
            </FormGroup>

            {#if isRecipeSelectionEnabled}
                <input name="recipeId" bind:value={recipeId} hidden/>
                <FormLink label="Recipe Selection"
                          text={recipe?.name ?? "(None)"}
                          onclick={onRecipeClick}/>

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

            {:else}
                <input name="itemId" bind:value={itemId} hidden/>
                <FormLink label="Item Selection"
                          text={item?.item.name ?? "(None)"}
                          onclick={onItemClick}/>

                {#if showPrepsSelect}
                    <FormGroup floating label="Prep">
                        <Input type="select" name="prepId" bind:value={prepId}>
                            {#each preps as prep}
                                <option value={prep?.id}>{prep?.name ?? '(None)'}</option>
                            {/each}
                        </Input>
                    </FormGroup>
                {/if}
                <div class="d-flex flex-column flex-sm-row justify-content-between">
                    <FormGroup floating label="Amount" class="flex-sm-grow-1">
                        <Input id="fractionInput" 
                               name="fraction" 
                               type="number" 
                               min={0} 
                               step={0.001} 
                               onfocus={onfocus} 
                               bind:value={fraction}>
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
            {/if}
</ModalCustom>