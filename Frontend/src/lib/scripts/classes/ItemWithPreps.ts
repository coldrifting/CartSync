import Item from "$lib/scripts/classes/Item.js";
import type Prep from "$lib/scripts/classes/Prep.ts";

class ItemWithPreps {
    item: Item = new Item();
    preps: (Prep | null)[] = [];
    hasExtraPreps: boolean = false;
}

export default ItemWithPreps;