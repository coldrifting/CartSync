import type {Cookies} from "@sveltejs/kit";
import {del} from "$lib/requests/requests.js";

export async function deleteItem(cookies: Cookies, itemId: string): Promise<void> {
    const url = `http://localhost:5164/api/items/${itemId}/delete`;
    const response = await del(cookies, url);
}