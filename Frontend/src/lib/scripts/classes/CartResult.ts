import type CartEntryAisle from "$lib/scripts/classes/CartEntryAisle.ts";

class CartResult {
    storeId: string = "";
    storeName: string = "";
    aisles: CartEntryAisle[] = [];
}

export default CartResult;