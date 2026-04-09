import type CartEntryItem from "$lib/scripts/classes/CartEntryItem.ts";

class CartEntryAisle {
    aisleId: string = "";
    aisleName: string = "";
    items: CartEntryItem[] = []; 
}

export default CartEntryAisle;