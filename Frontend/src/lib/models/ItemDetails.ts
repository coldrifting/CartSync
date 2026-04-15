import type Prep from "$lib/models/Prep.ts";
import type ItemLocation from "$lib/models/ItemLocation.ts";

class ItemDetails {
    id: string = "";
    name: string = "";
    temp: string = "";
    defaultUnitType: string = "";
    preps: Prep[] = [];
    locations: ItemLocation[] = [];
}

export default ItemDetails;