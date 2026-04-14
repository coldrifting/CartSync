import type CartEntryAisle from "$lib/models/CartEntryAisle.ts";

class CartResult {
    storeId: string = "";
    storeName: string = "";
    aisles: CartEntryAisle[] = [];
}

export default CartResult;