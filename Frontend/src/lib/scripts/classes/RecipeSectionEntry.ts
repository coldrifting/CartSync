import Item from "$lib/scripts/classes/Item.js";
import Prep from "$lib/scripts/classes/Prep.js";
import Amount from "$lib/scripts/classes/Amount.js";

class RecipeSectionEntry {
    recipeSectionEntryId: string = "";
    recipeSectionId: string = "";
    item: Item = new Item();
    prep: Prep | null = null;
    amount: Amount = new Amount();
}

export default RecipeSectionEntry;