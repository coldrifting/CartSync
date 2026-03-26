import {post} from "$lib/requests/requests.js";
import type {Cookies} from "@sveltejs/kit";

export async function addStore(cookies: Cookies, storeName: string): Promise<void> {
    const url = 'http://localhost:5164/api/stores/add';
    await post(cookies, url, { storeName: storeName });
}

export async function addAisle(cookies: Cookies, storeId: string, aisleName: string): Promise<void> {
    const url = `http://localhost:5164/api/stores/${storeId}/aisles/add`;
    await post(cookies, url, { aisleName: aisleName });
}

export async function addItem(cookies: Cookies, itemName: string): Promise<void> {
    const url = `http://localhost:5164/api/items/add`;
    await post(cookies, url, { itemName: itemName });
}