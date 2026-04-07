import type {Actions, PageServerLoad} from './$types';
import {getAllRecipes} from "$lib/scripts/requests/get.js";
import {getValue, getValueBoolean} from "$lib/scripts/requests/common.js";
import {addRecipe} from "$lib/scripts/requests/post.js";
import {deleteRecipe} from "$lib/scripts/requests/delete.js";
import {editRecipeName, editRecipeIsPinned} from "$lib/scripts/requests/patch.js";
import type Recipe from "$lib/scripts/classes/Recipe.ts";

export const load: PageServerLoad = async ({cookies}) => {
    const recipes: Recipe[] = await getAllRecipes(cookies);
    return {
        allRecipes: recipes,
        pinnedRecipes: recipes.filter(recipe => recipe.isPinned),
        unPinnedRecipes: recipes.filter(recipe => !recipe.isPinned)
    }
};

export const actions: Actions = {
    addRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeName: string = await getValue(data, 'inputAdd');
        await addRecipe(cookies, recipeName)
    },
    renameRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeId: string = await getValue(data, 'id');
        const recipeName: string = await getValue(data, 'inputRename');
        
        await editRecipeName(cookies, recipeId, recipeName);
    },
    toggleRecipePin: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeId: string = await getValue(data, 'id');
        const isPinned: boolean = await getValueBoolean(data, 'isPinned');
        
        await editRecipeIsPinned(cookies, recipeId, isPinned);
    },
    deleteRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const deleteRecipeId: string = await getValue(data, 'id');
        await deleteRecipe(cookies, deleteRecipeId);
    }
};