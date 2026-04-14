import Item from "$lib/models/Item.js";
import type Prep from "$lib/models/Prep.ts";

class ItemWithPreps {
    item: Item = new Item();
    preps: (Prep | null)[] = [];
    hasExtraPreps: boolean = false;
}

export default ItemWithPreps;