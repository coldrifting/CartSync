import type {Actions, PageServerLoad} from './$types';
import {getRecipe} from "$lib/scripts/requests/get.js";
import RecipeDetails from "$lib/scripts/classes/RecipeDetails.js";
import {getValue, getValueNumber} from "$lib/scripts/requests/common.js";
import {editRecipeInstruction, editRecipeInstructionOrder} from "$lib/scripts/requests/patch.js";
import {deleteRecipeInstruction} from "$lib/scripts/requests/delete.js";
import {addRecipeInstruction} from "$lib/scripts/requests/post.js";

let recipeId: string;

export const load: PageServerLoad = async ({cookies, params}) => {
    const recipe: RecipeDetails = await getRecipe(cookies, params.recipeId);
    
    recipeId = recipe.recipeId;
    
    return {
        recipe: recipe,
    }
};

export const actions: Actions = {
    addInstruction: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const contents: string = await getValue(data, 'stepContents');
        await addRecipeInstruction(cookies, recipeId, contents);
    },
    editInstruction: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const contents: string = await getValue(data, 'stepContents');
        const stepId: string = await getValue(data, 'stepId');
        await editRecipeInstruction(cookies, recipeId, stepId, contents);
    },
    deleteInstruction: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const instructionId: string = await getValue(data, 'id');
        await deleteRecipeInstruction(cookies, recipeId, instructionId);
    },
    reorderInstruction: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const instructionId: string = await getValue(data, 'id');
        const instructionSortOrder: number = await getValueNumber(data, 'instructionSortOrder');
        await editRecipeInstructionOrder(cookies, recipeId, instructionId, instructionSortOrder);
    }
}