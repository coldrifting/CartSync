import type {PageLoad} from './$types';
import RecipeDetails from "$lib/models/RecipeDetails.js";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch, params}) => {
    const recipe: RecipeDetails = await get(`/api/recipes/${params.recipeId}`, fetch)

    return {
        recipe: recipe,
    }
};