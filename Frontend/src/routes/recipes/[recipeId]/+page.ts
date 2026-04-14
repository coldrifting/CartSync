import type {PageLoad} from './$types';
import RecipeDetails from "$lib/models/RecipeDetails.js";
import type ItemDetails from "$lib/models/ItemDetails.ts";
import {AllValidItems} from "$lib/models/ValidItemsAndPreps.js";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch, params}) => {
    const recipe: RecipeDetails = await get(`/api/recipes/${params.recipeId}`, fetch)
    const items: ItemDetails[] = await get('/api/items', fetch);

    const validItemsAndPreps: AllValidItems = AllValidItems.fromData(recipe, items);

    return {
        recipe: recipe,
        items: items,
        validItemsAndPreps: validItemsAndPreps
    }
};