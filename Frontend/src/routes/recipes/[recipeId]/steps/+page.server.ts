import type {Actions, PageServerLoad} from './$types';
import {getRecipe} from "$lib/requests/get.js";
import RecipeDetails from "$lib/models/RecipeDetails.js";
import {getValue, getValueNumber} from "$lib/requests/common.js";
import {editRecipeStep, editRecipeStepOrder} from "$lib/requests/patch.js";
import {deleteRecipeStep} from "$lib/requests/delete.js";
import {addRecipeStep} from "$lib/requests/post.js";
import ErrorCustom from "$lib/models/ErrorCustom.js";

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
        try {
            await addRecipeStep(cookies, recipeId, contents);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    editStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const contents: string = await getValue(data, 'stepContents');
        const stepId: string = await getValue(data, 'stepId');
        try {
            await editRecipeStep(cookies, stepId, contents);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    deleteStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const stepId: string = await getValue(data, 'id');
        try {
            await deleteRecipeStep(cookies, stepId);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    reorderStep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const stepId: string = await getValue(data, 'id');
        const stepSortOrder: number = await getValueNumber(data, 'stepSortOrder');
        try {
            await editRecipeStepOrder(cookies, stepId, stepSortOrder);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    }
}