import type {PageLoad} from "./$types";
import type Store from "$lib/models/Store.ts";
import type Aisle from "$lib/models/Aisle.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch, params}) => {
    const stores: Store[] = await get<Store[]>('/api/stores', fetch);
    const store: Store = stores.filter(store => store.id === params.storeId)[0];
    const aisles: Aisle[] = await get<Aisle[]>(`/api/aisles?storeId=${params.storeId}`, fetch);
    
    return {
        store: store,
        aisles: aisles
    }
}