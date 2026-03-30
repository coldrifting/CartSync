import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, patch} from "$lib/requests/requests.js";

export async function editItemName(cookies: Cookies, itemId: string, itemName: string): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/edit`;
    await patch(cookies, url, "/ItemName", itemName);
}

export async function editItemTemp(cookies: Cookies, itemId: string, itemTemp: string): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/edit`;
    await patch(cookies, url, "/ItemTemp", itemTemp);
}

export async function editItemDefaultUnits(cookies: Cookies, itemId: string, defaultUnits: string): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/edit`;
    await patch(cookies, url, "/DefaultUnitType", defaultUnits);
}

export async function editItemPreps(cookies: Cookies, itemId: string, prepIds: string[]): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/edit`;
    await patch(cookies, url, "/PrepIds", prepIds);
}

export async function editPrepName(cookies: Cookies, prepId: string, prepName: string): Promise<void> {
    const url = `${apiBaseUrl}/preps/${prepId}/edit`;
    await patch(cookies, url, "/PrepName", prepName);
}

export async function editItemAisle(cookies: Cookies, itemId: string, location: LocationEdit | undefined): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/edit`
    await patch(cookies, url, "/Location", location);
}

export async function editStoreName(cookies: Cookies, storeId: string, storeName: string): Promise<void> {
    const url = `${apiBaseUrl}/stores/${storeId}/edit`;
    await patch(cookies, url, "/StoreName", storeName);
}