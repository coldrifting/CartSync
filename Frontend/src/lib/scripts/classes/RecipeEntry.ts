import Item from "$lib/scripts/classes/Item.js";
import Prep from "$lib/scripts/classes/Prep.js";
import Amount from "$lib/scripts/classes/Amount.js";

class RecipeEntry {
    id: string = "";
    item: Item = new Item();
    prep: Prep | null = null;
    amount: Amount = new Amount();
}

export default RecipeEntry;