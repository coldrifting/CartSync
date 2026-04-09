import type {Actions, PageServerLoad} from './$types';
import {getAllItems, getRecipe} from "$lib/requests/get.js";
import RecipeDetails from "$lib/models/RecipeDetails.js";
import type ItemDetails from "$lib/models/ItemDetails.ts";
import {getValue, getValueOrNull} from "$lib/requests/common.js";
import Amount from "$lib/models/Amount.js";
import Fraction from "$lib/models/Fraction.js";
import {AllValidItems} from "$lib/models/ValidItemsAndPreps.js";
import {addRecipeEntry, addRecipeSection} from "$lib/requests/post.js";
import {deleteRecipeEntry} from "$lib/requests/delete.js";
import {editRecipeEntry, editRecipeSectionName, editRecipeUrl} from "$lib/requests/patch.js";
import ErrorCustom from "$lib/models/ErrorCustom.js";

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
        try {
            await editRecipeSectionName(cookies, sectionId, newName);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    addRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const sectionId: string = await getValue(data, "recipeSectionId");
        const itemId: string = await getValue(data, "itemId");
        const prepId: string | null = await getValueOrNull(data, "prepId");
        const fractionString: string = await getValue(data, "fraction");
        const fraction: Fraction = Fraction.fromNumberString(fractionString);
        const unitType: string = await getValue(data, "unitType");

        const amount: Amount = {fraction: fraction, unitType: unitType} as Amount;

        try {
            if (sectionId === "") {
                // Create new section first
                const sectionName: string = await getValue(data, "recipeSectionName");
                const newSectionId: string = await addRecipeSection(cookies, recipeId, sectionName);

                await addRecipeEntry(cookies, newSectionId, itemId, prepId, amount);
            } else {
                await addRecipeEntry(cookies, sectionId, itemId, prepId, amount);
            }
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    editRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const entryId: string = await getValue(data, "recipeEntryId");

        const prepId: string | null = await getValueOrNull(data, "prepId");

        const fractionString: string = await getValue(data, "fraction");
        const fraction: Fraction = Fraction.fromNumberString(fractionString);

        const unitType: string = await getValue(data, "unitType");
        const amount: Amount = {fraction: fraction, unitType: unitType} as Amount;

        try {
            await editRecipeEntry(cookies, entryId, prepId, amount);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    deleteRecipeEntry: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const entryId: string = await getValue(data, "entryId");
        try {
            await deleteRecipeEntry(cookies, entryId);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    updateRecipeURL: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const newUrl: string = await getValue(data, "inputRename");

        try {
            await editRecipeUrl(cookies, recipeId, newUrl);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    }
}