<script lang="ts">
    import {tick} from "svelte";
    import {FormGroup, Input} from "@sveltestrap/sveltestrap";
    import type RecipeSection from "$lib/models/RecipeSection.ts";
    import type Prep from "$lib/models/Prep.ts";
    import UnitType from "$lib/models/UnitType.js";
    import FormLink from "$lib/components/FormLink.svelte";
    import {ValidItem, type AllValidItems} from "$lib/models/ValidItemsAndPreps.js";
    import ModalSearch from "$lib/components/modal/generic/ModalSearch.svelte";
    import ModalCustom from "$lib/components/modal/ModalCustom.svelte";
    import {post, postAndGetId} from "$lib/functions/requests.js";
    import type Amount from "$lib/models/Amount.ts";
    import Fraction from "$lib/models/Fraction.js";
    import {useQueryClient} from "@tanstack/svelte-query";

    interface Props {
        recipeId: string;
        sections: RecipeSection[];
        items: AllValidItems;
    }

    let {recipeId, sections, items}: Props = $props();

    const client = useQueryClient()
    
    let isOpen: boolean = $state(false);

    let sectionId: string | undefined = $derived(sections.length > 0 ? sections[0].id : undefined);
    let newSectionName: string = $derived.by(() => {
        sectionId;
        return "New Section";
    })

    let item: ValidItem | undefined = $derived.by(() => {
        sectionId; // Reset when section changes
        return undefined;
    });
    let itemId: string | undefined = $derived.by(() => {
        return item?.id;
    });

    let preps: (Prep | null)[] = $derived.by(() => {
        return item?.preps ?? []
    });
    let prepId: string | undefined = $derived.by(() => {
        return preps[0]?.id ?? undefined;
    });
    let showPrepsSelect: boolean = $derived.by(() => {
        return preps.length > 0 && (preps.length !== 1 || preps[0] !== null);
    });

    let unitType: string = $derived.by(() => {
        // Reset when item or section changes
        sectionId;
        return item?.defaultUnitType ?? UnitType.Types[0];
    });

    let fraction: number = $derived.by(() => {
        // Reset when item or section changes
        sectionId;
        item;
        return 1;
    });
    let isFractionValid: boolean = $derived( fraction > 0 );
    let amount: Amount = $derived.by(() => {
        return {fraction: Fraction.fromNumberString(fraction.toFixed(3)), unitType: unitType} as Amount;
    })
    
    let isSubmitDisabled: boolean = $derived(item === undefined || !isFractionValid || (sectionId === undefined && newSectionName.trim() === ""));
    
    $effect(() => {
        if (sectionId === undefined) {
            tick().then(() => {
                document.getElementById("sectionNameEdit")?.focus();
            })
        }
    })

    export function show() {
        sectionId = sections.length > 0 ? sections[0].id : undefined;
        item = undefined;
        prepId = undefined;
        fraction = 1;
        unitType = UnitType.Types[0];

        isOpen = true;
    }

    let isItemSearchOpen: boolean = $state(false);
    
    let modalSearch: ModalSearch<ValidItem>;
    
    let filteredItems: ValidItem[] = $derived(items.sections
        .filter(section => section.id === sectionId)[0].items);
    
    let isLoading: boolean = $state(false);
    async function onAdd() {
        isLoading = true;
        if (sectionId === undefined) {
            // Create new section first
            const newSectionId: string = await postAndGetId(`/api/recipes/sections/add?recipeId=${recipeId}`, {
                name: newSectionName
            });
            
            await post(`/api/recipes/entries/add?recipeSectionId=${newSectionId}`, {
                itemId: itemId, 
                prepId: prepId ?? null, 
                amount: amount
            });
        } else {
            await post(`/api/recipes/entries/add?recipeSectionId=${sectionId}`, {
                itemId: itemId, 
                prepId: prepId ?? null, 
                amount: amount
            });
        }
        await client.invalidateQueries({queryKey: ['recipes', recipeId]});
        isLoading = false;
        isOpen = false;
    }
    
    let firstElement: HTMLButtonElement | undefined = $state(undefined);
</script>

<ModalSearch bind:this={modalSearch} 
             bind:isOpen={isItemSearchOpen} 
             items={filteredItems} 
             itemType="Item"
             getItemName={(item) => item.name}
             bind:selectedItem={item}/>

<ModalCustom title="Add Recipe Entry"
             bind:isOpen
             bind:isLoading
             action={{label: "Add", action: onAdd}}
             actionIsDisabled={isSubmitDisabled}
             autoFocusElement={firstElement}>
    <FormGroup floating label="Recipe Section">
        <Input type="select"
               name="recipeSectionId"
               id="sectionIdSelect"
               bind:value={sectionId}>
            {#each sections as section}
                <option value={section.id}>{section.name}</option>
            {/each}
            <option value={undefined}>(New Section)</option>
        </Input>
    </FormGroup>

    <FormGroup hidden={sectionId !== undefined} floating label="New Recipe Section Name">
        <Input name="recipeSectionName" id="sectionNameEdit" bind:value={newSectionName}/>
    </FormGroup>

    <div>
        <FormLink text={item?.name ?? "(No Item Selected)"}
                  label="Ingredient Selection"
                  onclick={() => modalSearch.show(filteredItems)}
                  bind:element={firstElement}/>
    </div>

    <FormGroup hidden={!showPrepsSelect} floating label="Prep">
        <Input type="select"
               name="prepId"
               bind:value={prepId}>
            {#each preps as prep}
                <option value={prep?.id}>{prep?.name ?? '(None)'}</option>
            {/each}
        </Input>
    </FormGroup>
    <div class="d-flex flex-column flex-sm-row justify-content-between">
        <FormGroup floating label="Amount" class="flex-sm-grow-1">
            <Input name="fraction" type="number" inputmode="decimal" min={0.001} step={0.001} bind:value={fraction}>
            </Input>
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