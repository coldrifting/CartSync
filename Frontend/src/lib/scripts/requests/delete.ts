import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/scripts/requests/common.js";

export async function deleteItem(cookies: Cookies, itemId: string): Promise<void> {
    await del(cookies, `/items/${itemId}/delete`);
}

export async function deletePrep(cookies: Cookies, prepId: string): Promise<void> {
    await del(cookies, `/preps/${prepId}/delete`);
}

export async function deleteStore(cookies: Cookies, storeId: string): Promise<void> {
    await del(cookies, `/stores/${storeId}/delete`);
}

export async function deleteAisle(cookies: Cookies, storeId: string, aisleId: string): Promise<void> {
    await del(cookies, `/stores/${storeId}/aisles/${aisleId}/delete`);
}

export async function deleteRecipe(cookies: Cookies, recipeId: string): Promise<void> {
    await del(cookies, `/recipes/${recipeId}/delete`);
}

export async function deleteRecipeInstruction(cookies: Cookies, recipeId: string, instructionId: string): Promise<void> {
    await del(cookies, `/recipes/${recipeId}/instructions/${instructionId}/delete`);
}

export async function deleteRecipeEntry(cookies: Cookies, recipeId: string, sectionId: string, entryId: string): Promise<void> {
    await del(cookies, `/recipes/${recipeId}/sections/${sectionId}/entries/${entryId}/delete`);
}

async function del(cookies: Cookies, url: string): Promise<void> {
    const token = getToken(cookies);
    const response = await fetch(apiBaseUrl + url, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    await checkForErrors(cookies, response);
}