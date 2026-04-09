import type {Actions, PageServerLoad} from './$types';
import {getAllRecipes} from "$lib/requests/get.js";
import {getValue, getValueBoolean} from "$lib/requests/common.js";
import {addRecipe} from "$lib/requests/post.js";
import {deleteRecipe} from "$lib/requests/delete.js";
import {editRecipeName, editRecipeIsPinned} from "$lib/requests/patch.js";
import type Recipe from "$lib/models/Recipe.ts";
import ErrorCustom from "$lib/models/ErrorCustom.js";

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
        try {
            await addRecipe(cookies, recipeName)
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    renameRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeId: string = await getValue(data, 'id');
        const recipeName: string = await getValue(data, 'inputRename');
        try {
            await editRecipeName(cookies, recipeId, recipeName);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    toggleRecipePin: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeId: string = await getValue(data, 'id');
        const isPinned: boolean = await getValueBoolean(data, 'isPinned');
        try {
            await editRecipeIsPinned(cookies, recipeId, isPinned);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    deleteRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const deleteRecipeId: string = await getValue(data, 'id');
        try {
            await deleteRecipe(cookies, deleteRecipeId);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    }
};