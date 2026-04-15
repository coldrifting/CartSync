import Item from "$lib/models/Item.js";
import ItemPrep from "$lib/models/ItemPrep.js";

class ItemPrepDetails {
    item: Item = new Item();
    allPreps: ItemPrep[] = [];
}

export default ItemPrepDetails;