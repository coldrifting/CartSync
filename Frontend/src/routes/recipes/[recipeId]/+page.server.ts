import type {Actions, PageServerLoad} from './$types';
import {getAllItems, getRecipe} from "$lib/scripts/requests/get.js";
import RecipeDetails from "$lib/scripts/classes/RecipeDetails.js";
import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";
import {getValue, getValueOrNull, getValueOrUndefined} from "$lib/scripts/requests/common.js";
import Amount from "$lib/scripts/classes/Amount.js";
import Fraction from "$lib/scripts/classes/Fraction.js";
import {AllValidItems} from "$lib/scripts/classes/ValidItemsAndPreps.js";
import {addRecipeEntry, addRecipeSection} from "$lib/scripts/requests/post.js";
import {deleteRecipeEntry} from "$lib/scripts/requests/delete.js";
import {editRecipeEntry, editRecipeSectionName, editRecipeUrl} from "$lib/scripts/requests/patch.js";

let recipeId: string;

export const load: PageServerLoad = async ({cookies, params}) => {
    const recipe: RecipeDetails = await getRecipe(cookies, params.recipeId);
    const items: ItemDetails[] = await getAllItems(cookies);
    
    const validItemsAndPreps: AllValidItems = AllValidItems.fromData(recipe, items);
    
    recipeId = recipe.id;
    
    return {
        recipe: recipe,
        items: items,
        validItemsAndPreps: validItemsAndPreps
    }
};

export const actions: Actions = {
    renameRecipeSection: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const sectionId: string = await getValue(data, "id");
        const newName: string = await getValue(data, "inputRename");
        
        await editRecipeSectionName(cookies, sectionId, newName);
    },
    addRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const sectionId: string = await getValue(data, "recipeSectionId");
        const itemId: string = await getValue(data, "itemId");
        const prepId: string | null = await getValueOrUndefined(data, "prepId") ?? null;
        const fraction: string = await getValue(data, "fraction");
        const unitType: string = await getValue(data, "unitType");
        
        const amount: Amount = {fraction: Fraction.fromString(fraction), unitType: unitType} as Amount;
        
        if (sectionId === "") {
            // Create new section first
            const sectionName: string = await getValue(data, "recipeSectionName");
            const newSectionId: string = await addRecipeSection(cookies, recipeId, sectionName);
            
            await addRecipeEntry(cookies, newSectionId, itemId, prepId, amount);
        }
        else {
            await addRecipeEntry(cookies, sectionId, itemId, prepId, amount);
        }
    },
    editRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const entryId: string = await getValue(data, "recipeEntryId");
        
        const prepId: string | null = await getValueOrNull(data, "prepId");
        
        const fraction: string = await getValue(data, "fraction");
        const unitType: string = await getValue(data, "unitType");
        const amount: Amount = {fraction: Fraction.fromString(fraction), unitType: unitType} as Amount;
        
        await editRecipeEntry(cookies, entryId, prepId, amount);
    },
    deleteRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const entryId: string = await getValue(data, "entryId");
        
        await deleteRecipeEntry(cookies, entryId);
    },
    editRecipeUrl: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const newUrl: string = await getValue(data, "inputRename");
        
        await editRecipeUrl(cookies, recipeId, newUrl);
    }
}