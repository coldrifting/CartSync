import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, del} from "$lib/requests/requests.js";

export async function deleteItem(cookies: Cookies, itemId: string): Promise<void> {
    const url = `${apiBaseUrl}/items/${itemId}/delete`;
    await del(cookies, url);
}

export async function deletePrep(cookies: Cookies, prepId: string): Promise<void> {
    const url = `${apiBaseUrl}/preps/${prepId}/delete`;
    await del(cookies, url);
}