import Item from "$lib/scripts/classes/Item.js";
import type Prep from "$lib/scripts/classes/Prep.ts";
import AmountGroup from "$lib/scripts/classes/AmountGroup.js";

class CartEntryItem {
    item: Item = new Item();
    prep?: Prep | undefined = undefined;
    bay: string = "";
    amounts: AmountGroup = new AmountGroup();
    isChecked: boolean = false;
}

export default CartEntryItem;