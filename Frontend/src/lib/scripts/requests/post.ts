import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/scripts/requests/common.js";
import type Amount from "$lib/scripts/classes/Amount.ts";
import type RecipeSection from "$lib/scripts/classes/RecipeSection.ts";

export async function addStore(cookies: Cookies, storeName: string): Promise<void> {
    await post(cookies, `/stores/add`, { storeName: storeName });
}

export async function addAisle(cookies: Cookies, storeId: string, aisleName: string): Promise<void> {
    await post(cookies, `/stores/${storeId}/aisles/add`, { aisleName: aisleName });
}

export async function addItem(cookies: Cookies, itemName: string): Promise<void> {
    await post(cookies, `/items/add`, { itemName: itemName });
}

export async function addPrep(cookies: Cookies, prepName: string): Promise<void> {
    await post(cookies, `/preps/add`, { prepName: prepName });
}

export async function setCurrentStore(cookies: Cookies, storeId: string): Promise<void> {
    await post(cookies, `/stores/${storeId}/select`, {});
}

export async function addRecipe(cookies: Cookies, recipeName: string): Promise<void> {
    await post(cookies, `/recipes/add`, { recipeName: recipeName });
}

export async function addRecipeSection(cookies: Cookies, recipeId: string, recipeSectionName: string): Promise<string> {
    let recipeSection: RecipeSection = await postResults<RecipeSection>(cookies, `/recipes/${recipeId}/sections/add`, { 
        recipeSectionName: recipeSectionName
    });
    
    return recipeSection.recipeSectionId;
}

export async function addRecipeEntry(cookies: Cookies, recipeId: string, recipeSectionId: string, itemId: string, prepId: string | null, amount: Amount): Promise<void> {
    await post(cookies, `/recipes/${recipeId}/sections/${recipeSectionId}/entries/add`, { 
        itemId: itemId,
        prepId: prepId,
        amount: amount
    });
}

async function postResults<T>(cookies: Cookies, url: string, body: any): Promise<T> {
    const response: Response = await post(cookies, url, body) as Response;
    return await response.json();
}

async function post(cookies: Cookies, url: string, body: any): Promise<Response | void> {
    const token = getToken(cookies);
    const response = await fetch(apiBaseUrl + url, {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    await checkForErrors(cookies, response);
    if (response.status === 204) {
        return;
    }
    
    return response;
}