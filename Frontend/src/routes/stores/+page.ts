import type {PageLoad} from "./$types";
import type Store from "$lib/models/Store.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch}) => {
    const stores: Store[] = await get<Store[]>('/api/stores', fetch)
    return {
        selectedStore: stores.filter(store => store.isSelected)[0],
        stores: stores
    }
}