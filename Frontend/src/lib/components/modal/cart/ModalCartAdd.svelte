<script lang="ts">
    import {enhance} from '$app/forms';
    import {trapFocus} from 'trap-focus-svelte'
    import {Modal, ModalFooter, FormGroup, Input, Button} from "@sveltestrap/sveltestrap";
    import type {SubmitFunction} from "@sveltejs/kit";
    import type Prep from "$lib/models/Prep.ts";
    import UnitType from "$lib/models/UnitType.js";
    import type Recipe from "$lib/models/Recipe.ts";
    import FormLink from "$lib/components/FormLink.svelte";
    import ItemWithPreps from "$lib/models/ItemWithPreps.js";
    import ModalHeaderCustom from "$lib/components/modal/ModalHeaderCustom.svelte";
    import ModalSearch from "$lib/components/modal/generic/ModalSearch.svelte";

    interface Props {
        formUrl: string;
        remainingRecipes: Recipe[];
        remainingItems: ItemWithPreps[];
    }

    let {formUrl, remainingRecipes, remainingItems}: Props = $props();

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

    export const show = () => {
        recipe = undefined;
        item = undefined;
        prepId = undefined;
        isOpen = true;
    }

    const submitFunction: SubmitFunction = () => {
        return async ({update}) => {
            isOpen = false
            await update({reset: false});
        };
    };

    const getRecipeName = (recipe: Recipe) => {
        return recipe.name;
    }
    const getItemName = (item: ItemWithPreps) => {
        return item.item.name;
    }
    
    const isDisabled = () => {
        if (isRecipeSelectionEnabled) {
            return recipe === undefined || recipeQuantity <= 0;
        }
        else {
            return item === undefined || !isFractionValid;
        }
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
    let allowEscapeKey: boolean = $derived(!isItemSearchOpen && !isRecipeSelectionEnabled);

    let modalSearchRecipe: ModalSearch<Recipe>;
    let modalSearchItem: ModalSearch<ItemWithPreps>;
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

<Modal body
       isOpen={isOpen}
       toggle={() => isOpen = !isOpen}
       autoFocus={false}
       on:open={() => document.getElementById("isRecipeTypeInput")?.focus()}
       keyboard={allowEscapeKey}
       centered={true}>
    <form method="POST"
          action="?/{formUrl}"
          id={formUrl}
          use:trapFocus={false}
          use:enhance={submitFunction}>
        <ModalHeaderCustom title="Add Cart {isRecipeSelectionEnabled ? 'Recipe' : 'Item'}" bind:isOpen={isOpen} />
        <div>
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

        </div>
        <ModalFooter>
            <Button color="secondary" type="button" onclick={() => isOpen = false}>Cancel</Button>
            <Button color="primary" type="submit" disabled={isDisabled()}>Add</Button>
        </ModalFooter>
    </form>
</Modal>