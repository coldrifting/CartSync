import type {Actions, PageServerLoad} from './$types';
import {getRecipe} from "$lib/scripts/requests/get.js";
import RecipeDetails from "$lib/scripts/classes/RecipeDetails.js";
import {getValue, getValueNumber} from "$lib/scripts/requests/common.js";
import {editRecipeStep, editRecipeStepOrder} from "$lib/scripts/requests/patch.js";
import {deleteRecipeStep} from "$lib/scripts/requests/delete.js";
import {addRecipeStep} from "$lib/scripts/requests/post.js";

let recipeId: string;

export const load: PageServerLoad = async ({cookies, params}) => {
    const recipe: RecipeDetails = await getRecipe(cookies, params.recipeId);
    
    recipeId = recipe.id;
    
    return {
        recipe: recipe,
    }
};

export const actions: Actions = {
    addStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const contents: string = await getValue(data, 'stepContents');
        await addRecipeStep(cookies, recipeId, contents);
    },
    editStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const contents: string = await getValue(data, 'stepContents');
        const stepId: string = await getValue(data, 'stepId');
        await editRecipeStep(cookies, stepId, contents);
    },
    deleteStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const stepId: string = await getValue(data, 'id');
        await deleteRecipeStep(cookies, stepId);
    },
    reorderStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const stepId: string = await getValue(data, 'id');
        const stepSortOrder: number = await getValueNumber(data, 'stepSortOrder');
        await editRecipeStepOrder(cookies, stepId, stepSortOrder);
    }
}