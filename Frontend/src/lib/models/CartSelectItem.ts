import Amount from "$lib/models/Amount.js";
import Item from "$lib/models/Item.js";
import type Prep from "$lib/models/Prep.ts";

class CartSelectItem {
    item: Item = new Item();
    prep: Prep | null = null;
    amount: Amount = new Amount();
}

export default CartSelectItem;