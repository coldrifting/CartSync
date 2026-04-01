import type {Actions, PageServerLoad} from './$types';
import {getRecipe} from "$lib/scripts/requests/get.js";
import RecipeDetails from "$lib/scripts/classes/RecipeDetails.js";

export const load: PageServerLoad = async ({cookies, params}) => {
    const recipe: RecipeDetails = await getRecipe(cookies, params.recipeId);
    
    return {
        recipe: recipe
    }
};

export const actions: Actions = {
    addRecipeItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        // TODO
    }
}