import Amount from "$lib/scripts/classes/Amount.js";
import Item from "$lib/scripts/classes/Item.js";
import type Prep from "$lib/scripts/classes/Prep.ts";

class CartSelectItem {
    item: Item = new Item();
    prep: Prep | null = null;
    amount: Amount = new Amount();
}

export default CartSelectItem;