import type Prep from "$lib/scripts/classes/Prep.ts";
import type ItemLocation from "$lib/scripts/classes/ItemLocation.ts";

class ItemDetails {
    id: string = "";
    name: string = "";
    temp: string = "";
    defaultUnitType: string = "";
    preps: Prep[] = [];
    location: ItemLocation | null = null;
}

export default ItemDetails;