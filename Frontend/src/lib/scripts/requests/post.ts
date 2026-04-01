import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/scripts/requests/common.js";

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

async function post(cookies: Cookies, url: string, body: any): Promise<void> {
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
}