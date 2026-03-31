import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, del} from "$lib/requests/requests.js";

export async function deleteItem(cookies: Cookies, itemId: string): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/delete`;
    await del(cookies, url);
}

export async function deletePrep(cookies: Cookies, prepId: string): Promise<void> {
    const url = `${apiBaseUrl}/preps/${prepId}/delete`;
    await del(cookies, url);
}

export async function deleteStore(cookies: Cookies, storeId: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/${storeId}/delete`;
    await del(cookies, url);
}

export async function deleteAisle(cookies: Cookies, storeId: string, aisleId: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/${storeId}/aisles/${aisleId}/delete`;
    await del(cookies, url);
}

export async function deleteRecipe(cookies: Cookies, recipeId: string): Promise<void> {
    const url = `${apiBaseUrl}/recipes/${recipeId}/delete`;
    await del(cookies, url);
}