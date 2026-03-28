import {apiBaseUrl, post} from "$lib/requests/requests.js";
import type {Cookies} from "@sveltejs/kit";

export async function addStore(cookies: Cookies, storeName: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/add`;
    await post(cookies, url, { storeName: storeName });
}

export async function addAisle(cookies: Cookies, storeId: string, aisleName: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/${storeId}/aisles/add`;
    await post(cookies, url, { aisleName: aisleName });
}

export async function addItem(cookies: Cookies, itemName: string): Promise<void> {
    const url = `${apiBaseUrl}/items/add`;
    await post(cookies, url, { itemName: itemName });
}

export async function addPrep(cookies: Cookies, prepName: string): Promise<void> {
    const url = `${apiBaseUrl}/preps/add`;
    await post(cookies, url, { prepName: prepName });
}

export async function setCurrentStore(cookies: Cookies, storeId: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/${storeId}/select`;
    await post(cookies, url, {});
}
