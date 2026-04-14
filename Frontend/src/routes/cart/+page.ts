import CartSelect from "$lib/models/CartSelect.js";
import {get} from "$lib/functions/requests.js";

export async function load({ fetch }) {
    const cartSelection: CartSelect = await get<CartSelect>('/api/cart/selection', fetch);
    
    return {
        items: cartSelection.items,
        recipes: cartSelection.recipes,
        remainingRecipes: cartSelection.remainingRecipes,
        remainingItems: cartSelection.remainingItems,
    }
}