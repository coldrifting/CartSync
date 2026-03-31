import type {Actions, PageServerLoad} from './$types';
import {getRecipe} from "$lib/requests/get.js";

export const load: PageServerLoad = async ({cookies, params}) => {
    const recipe: Recipe = await getRecipe(cookies, params.recipeId);
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