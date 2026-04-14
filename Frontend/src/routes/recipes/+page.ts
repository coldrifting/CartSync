import type {PageLoad} from './$types';
import type Recipe from "$lib/models/Recipe.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch}) => {
    const recipes: Recipe[] = await get<Recipe[]>('/api/recipes', fetch);
    return {
        allRecipes: recipes,
        pinnedRecipes: recipes.filter(recipe => recipe.isPinned),
        unPinnedRecipes: recipes.filter(recipe => !recipe.isPinned)
    }
};