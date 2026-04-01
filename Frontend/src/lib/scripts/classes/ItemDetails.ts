import type Prep from "$lib/scripts/classes/Prep.ts";
import type ItemLocation from "$lib/scripts/classes/ItemLocation.ts";

class ItemDetails {
    itemId: string = "";
    itemName: string = "";
    itemTemp: string = "";
    defaultUnitType: string = "";
    preps: Prep[] = [];
    location: ItemLocation | null = null;
}

export default ItemDetails;