import Item from "$lib/models/Item.js";
import Prep from "$lib/models/Prep.js";
import Amount from "$lib/models/Amount.js";

class RecipeEntry {
    id: string = "";
    item: Item = new Item();
    prep: Prep | null = null;
    amount: Amount = new Amount();
}

export default RecipeEntry;