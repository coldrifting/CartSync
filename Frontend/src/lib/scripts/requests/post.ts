import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken, isContentImage} from "$lib/scripts/requests/common.js";
import type Amount from "$lib/scripts/classes/Amount.ts";
import type RecipeSection from "$lib/scripts/classes/RecipeSection.ts";

export async function addStore(cookies: Cookies, storeName: string): Promise<void> {
    await post(cookies, `/stores/add`, { Name: storeName });
}

export async function addAisle(cookies: Cookies, storeId: string, aisleName: string): Promise<void> {
    await post(cookies, `/aisles/add?storeId=${storeId}`, { Name: aisleName });
}

export async function addItem(cookies: Cookies, itemName: string): Promise<void> {
    await post(cookies, `/items/add`, { Name: itemName });
}

export async function addPrep(cookies: Cookies, prepName: string): Promise<void> {
    await post(cookies, `/preps/add`, { Name: prepName });
}

export async function setCurrentStore(cookies: Cookies, storeId: string): Promise<void> {
    await post(cookies, `/stores/${storeId}/select`, {});
}

export async function addRecipe(cookies: Cookies, recipeName: string): Promise<void> {
    await post(cookies, `/recipes/add`, { Name: recipeName });
}

export async function addRecipeSection(cookies: Cookies, recipeId: string, recipeSectionName: string): Promise<string> {
    let recipeSection: RecipeSection = await postResults<RecipeSection>(cookies, `/recipes/sections/add?recipeId=${recipeId}`, { 
        Name: recipeSectionName
    });
    
    return recipeSection.id;
}

export async function addRecipeEntry(cookies: Cookies, recipeSectionId: string, itemId: string, prepId: string | null, amount: Amount): Promise<void> {
    await post(cookies, `/recipes/entries/add?recipeSectionId=${recipeSectionId}`, { 
        itemId: itemId,
        prepId: prepId,
        amount: amount
    });
}

export async function addRecipeStep(cookies: Cookies, recipeId: string, content: string): Promise<void> {
    await post(cookies, `/recipes/steps/add?recipeId=${recipeId}`, { 
        Content: content,
        isImage: isContentImage(content)
    });
}

export async function generateCart(cookies: Cookies): Promise<void> {
    await post(cookies, `/cart/generate`, { });
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