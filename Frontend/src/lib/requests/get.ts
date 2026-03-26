import {get} from "$lib/requests/requests.js";
import type ItemUsagesReport from "$lib/types/ItemUsagesReport.js";
import type {Cookies} from "@sveltejs/kit";

export async function getAllStores(cookies: Cookies): Promise<Store[]> {
    const url = 'http://localhost:5164/api/stores';
    const response = await get(cookies, url);
    return await response.json();
}

export async function getAllAisles(cookies: Cookies, storeId: string): Promise<Aisle[]> {
    const url = `http://localhost:5164/api/stores/${storeId}/aisles`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getItem(cookies: Cookies, itemId: string): Promise<Ingredient> {
    const url = `http://localhost:5164/api/items/${itemId}`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getItemUsages(cookies: Cookies, itemId: string): Promise<ItemUsagesReport> {
    const url = `http://localhost:5164/api/items/${itemId}/usages`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getAllItemsByStore(cookies: Cookies): Promise<IngredientByStore[]> {
    
    const url = 'http://localhost:5164/api/items';
    const response = await get(cookies, url);
    return await response.json();
}

export async function getAllPreps(cookies: Cookies): Promise<Prep[]> {
    const url = 'http://localhost:5164/api/preps';
    const response = await get(cookies, url);
    return await response.json();
}