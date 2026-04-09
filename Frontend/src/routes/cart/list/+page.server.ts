import type {Actions, PageServerLoad} from "./$types";
import {getCart} from "$lib/scripts/requests/get.js";
import type CartResult from "$lib/scripts/classes/CartResult.ts";
import {getValue, getValueBoolean} from "$lib/scripts/requests/common.js";
import {toggleCartItemChecked} from "$lib/scripts/requests/put.js";

export const load: PageServerLoad = async ({cookies}) => {
    const cart: CartResult = await getCart(cookies);
    
    return {
        cart: cart
    }
}

export const actions: Actions = {
    toggleCartItemChecked: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const id: string = await getValue(data, "id");
        const isChecked: boolean = await getValueBoolean(data, 'isChecked');
        
        const itemId: string = id.split('/')[0];
        const prepId: string | null = id.split('/')[1] === "(None)" ? null : id.split('/')[1];
        
        await toggleCartItemChecked(cookies, itemId, prepId, isChecked)
    }
}