import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/requests/common.js";
import type Store from "$lib/models/Store.ts";
import type Aisle from "$lib/models/Aisle.ts";
import type ItemDetails from "$lib/models/ItemDetails.ts";
import type ItemUsagesReport from "$lib/models/ItemUsagesReport.ts";
import type Prep from "$lib/models/Prep.ts";
import type PrepUsagesReport from "$lib/models/PrepUsagesReport.ts";
import type Recipe from "$lib/models/Recipe.ts";
import type RecipeDetails from "$lib/models/RecipeDetails.ts";
import type CartSelect from "$lib/models/CartSelect.ts";
import type CartResult from "$lib/models/CartResult.ts";

export async function getAllStores(cookies: Cookies): Promise<Store[]> {
    return await get<Store[]>(cookies, `/stores`);
}

export async function getAllAisles(cookies: Cookies, storeId: string): Promise<Aisle[]> {
    return await get<Aisle[]>(cookies, `/aisles?storeId=${storeId}`);
}

export async function getItem(cookies: Cookies, itemId: string): Promise<ItemDetails> {
    return await get<ItemDetails>(cookies, `/items/${itemId}`);
}

export async function getItemUsages(cookies: Cookies, itemId: string): Promise<ItemUsagesReport> {
    return await get<ItemUsagesReport>(cookies, `/items/${itemId}/usages`);
}

export async function getAllItems(cookies: Cookies): Promise<ItemDetails[]> {
    return await get<ItemDetails[]>(cookies, '/items');
}

export async function getAllPreps(cookies: Cookies): Promise<Prep[]> {
    return await get<Prep[]>(cookies, '/preps');
}

export async function getPrepUsages(cookies: Cookies, prepId: string): Promise<PrepUsagesReport> {
    return await get<PrepUsagesReport>(cookies, `/preps/${prepId}/usages`);
}

export async function getAllRecipes(cookies: Cookies): Promise<Recipe[]> {
    return await get<Recipe[]>(cookies, `/recipes`);
}

export async function getRecipe(cookies: Cookies, recipeId: string): Promise<RecipeDetails> {
    return await get<RecipeDetails>(cookies, `/recipes/${recipeId}`);
}

export async function getCartSelection(cookies: Cookies): Promise<CartSelect> {
    return await get<CartSelect>(cookies, `/cart/selection`);
}

export async function getCart(cookies: Cookies): Promise<CartResult> {
    return await get<CartResult>(cookies, `/cart`);
}

async function get<T>(cookies: Cookies, url: string): Promise<T> {
    const token = getToken(cookies);
    const response = await fetch(apiBaseUrl + url, {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }
    });
    await checkForErrors(cookies, response);
    return await response.json()
}