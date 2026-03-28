import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, get} from "$lib/requests/requests.js";
import ItemUsagesReport from "$lib/types/ItemUsagesReport.js";
import PrepUsagesReport from "$lib/types/PrepUsagesReport.js";

export async function getAllStores(cookies: Cookies): Promise<Store[]> {
    const url = `${apiBaseUrl}/stores`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getAllAisles(cookies: Cookies, storeId: string): Promise<Aisle[]> {
    const url = `${apiBaseUrl}/stores/${storeId}/aisles`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getItem(cookies: Cookies, itemId: string): Promise<IngredientByStore> {
    const url = `${apiBaseUrl}/items/${itemId}`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getItemUsages(cookies: Cookies, itemId: string): Promise<ItemUsagesReport> {
    const url = `${apiBaseUrl}/items/${itemId}/usages`;
    const response = await get(cookies, url);
    const plainObj: ItemUsagesReport = await response.json();
    const result = new ItemUsagesReport();
    Object.assign(result, plainObj);
    return result;
}

export async function getAllItems(cookies: Cookies): Promise<IngredientByStore[]> {
    
    const url = `${apiBaseUrl}/items`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getAllPreps(cookies: Cookies): Promise<Prep[]> {
    const url = `${apiBaseUrl}/preps`;
    const response = await get(cookies, url);
    return await response.json();
}

export async function getPrepUsages(cookies: Cookies, prepId: string): Promise<PrepUsagesReport> {
    const url = `${apiBaseUrl}/preps/${prepId}/usages`;
    const response = await get(cookies, url);
    const plainObj: PrepUsagesReport = await response.json();
    const result = new PrepUsagesReport();
    Object.assign(result, plainObj);
    return result;
}