import type CartEntryItem from "$lib/models/CartEntryItem.ts";

class CartEntryAisle {
    aisleId: string = "";
    aisleName: string = "";
    items: CartEntryItem[] = []; 
}

export default CartEntryAisle;