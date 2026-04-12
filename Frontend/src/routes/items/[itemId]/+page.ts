import type {PageLoad} from './$types';
import type Store from "$lib/models/Store.ts";
import {get} from "$lib/functions/requests.js";
import type Aisle from "$lib/models/Aisle.js";
import type ItemDetails from "$lib/models/ItemDetails.ts";

export const load: PageLoad = async ({fetch, params}) => {
    const stores: Store[] = await get<Store[]>('/api/stores', fetch);
    const selectedStore: Store = stores.filter(store => store.isSelected)[0];
    
    const [aisles, item] = await Promise.all([
        get<Aisle[]>(`/api/aisles?storeId=${selectedStore.id}`, fetch),
        get<ItemDetails>(`/api/items/${params.itemId}`, fetch)
    ]);
    
    return {
        stores: stores,
        selectedStore: selectedStore,
        aisles: aisles,
        selectedPreps: item.preps,
        item: item
    }
};