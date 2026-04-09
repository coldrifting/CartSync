import Item from "$lib/models/Item.js";
import type Prep from "$lib/models/Prep.ts";
import AmountGroup from "$lib/models/AmountGroup.js";

class CartEntryItem {
    item: Item = new Item();
    prep?: Prep | undefined = undefined;
    bay: string = "";
    amounts: AmountGroup = new AmountGroup();
    isChecked: boolean = false;
}

export default CartEntryItem;