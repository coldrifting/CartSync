import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/scripts/requests/common.js";
import type Amount from "$lib/scripts/classes/Amount.ts";

export async function editCartRecipe(cookies: Cookies, recipeId: string, recipeQuantity: number): Promise<void> {
    await put(cookies, `/cart/selection/recipes/${recipeId}/edit`, { 
        Quantity: recipeQuantity
    });
}

export async function editCartItem(cookies: Cookies, itemId: string, prepId: string | null, amount: Amount): Promise<void> {
    await put(cookies, `/cart/selection/items/${itemId}/edit` + (prepId !== null ? `?prepId=${prepId}` : ''), { 
        Amount: amount
    });
}

async function putResults<T>(cookies: Cookies, url: string, body: any): Promise<T> {
    const response: Response = await put(cookies, url, body) as Response;
    return await response.json();
}

async function put(cookies: Cookies, url: string, body: any): Promise<Response | void> {
    const token = getToken(cookies);
    const response = await fetch(apiBaseUrl + url, {
        method: 'PUT',
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