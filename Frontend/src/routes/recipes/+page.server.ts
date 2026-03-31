import type {Actions, PageServerLoad} from './$types';
import {getAllRecipes} from "$lib/requests/get.js";
import {getValue, getValueBoolean} from "$lib/requests/requests.js";
import {addRecipe} from "$lib/requests/post.js";
import {deleteRecipe} from "$lib/requests/delete.js";
import {editRecipeName, editRecipeIsPinned} from "$lib/requests/patch.js";

export const load: PageServerLoad = async ({cookies}) => {
    const recipes: RecipeMinimal[] = await getAllRecipes(cookies);
    return {
        allRecipes: recipes,
        pinnedRecipes: recipes.filter(r => r.isPinned),
        unPinnedRecipes: recipes.filter(r => !r.isPinned)
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