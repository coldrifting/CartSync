import type {PageLoad} from "./$types";
import type CartResult from "$lib/models/CartResult.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch}) => {
    const cart: CartResult = await get<CartResult>("/api/cart", fetch);
    
    return {
        cart: cart
    }
}