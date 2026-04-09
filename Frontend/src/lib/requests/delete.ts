import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/requests/common.js";

export async function deleteItem(cookies: Cookies, itemId: string): Promise<void> {
    await del(cookies, `/items/${itemId}/delete`);
}

export async function deletePrep(cookies: Cookies, prepId: string): Promise<void> {
    await del(cookies, `/preps/${prepId}/delete`);
}

export async function deleteStore(cookies: Cookies, storeId: string): Promise<void> {
    await del(cookies, `/stores/${storeId}/delete`);
}

export async function deleteAisle(cookies: Cookies, aisleId: string): Promise<void> {
    await del(cookies, `/aisles/${aisleId}/delete`);
}

export async function deleteRecipe(cookies: Cookies, recipeId: string): Promise<void> {
    await del(cookies, `/recipes/${recipeId}/delete`);
}

export async function deleteRecipeStep(cookies: Cookies, stepId: string): Promise<void> {
    await del(cookies, `/recipes/steps/${stepId}/delete`);
}

export async function deleteRecipeEntry(cookies: Cookies, entryId: string): Promise<void> {
    await del(cookies, `/recipes/entries/${entryId}/delete`);
}

export async function deleteCartRecipe(cookies: Cookies, recipeId: string): Promise<void> {
    await del(cookies, `/cart/selection/recipes/${recipeId}/delete`);
}

export async function deleteCartItem(cookies: Cookies, itemId: string, prepId: string | null): Promise<void> {
    await del(cookies, `/cart/selection/items/${itemId}/delete` + (prepId !== null ? `?prepId=${prepId}` : ''));
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