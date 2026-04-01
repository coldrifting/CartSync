import type {Actions, PageServerLoad} from './$types';
import {getAllAisles, getAllStores, getItem} from "$lib/scripts/requests/get.js";
import {getValue} from "$lib/scripts/requests/common.js";
import {editItemAisle, editItemDefaultUnits, editItemTemp} from "$lib/scripts/requests/patch.js";
import {setCurrentStore} from "$lib/scripts/requests/post.js";
import type Store from "$lib/scripts/classes/Store.ts";

export const load: PageServerLoad = async ({params, cookies}) => {
    const stores: Store[] = await getAllStores(cookies);
    const selectedStore: Store = stores.filter((s: Store): boolean => s.isSelected)[0];
    
    const [aisles, item] = await Promise.all([
        getAllAisles(cookies, selectedStore.storeId),
        getItem(cookies, params.itemId)
    ]);
    
    return {
        stores: stores,
        selectedStore: selectedStore,
        aisles: aisles,
        selectedPreps: item.preps,
        item: item
    }
};

export const actions: Actions = {
    editItemTemp: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemTemp: string = await getValue(data, 'itemTemp');
        const itemTempItemId: string = await getValue(data, 'itemId');
        await editItemTemp(cookies, itemTempItemId, itemTemp);
    },
    editItemDefaultUnits: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemDefaultUnits: string = await getValue(data, 'itemDefaultUnits');
        const itemDefaultUnitsItemId: string = await getValue(data, 'itemId');
        await editItemDefaultUnits(cookies, itemDefaultUnitsItemId, itemDefaultUnits);
    },
    editCurrentStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'storeId');
        await setCurrentStore(cookies, storeId);
    },
    editItemAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemId: string = await getValue(data, 'itemId');
        const aisleId: string = await getValue(data, 'aisleId');
        const bay: string = await getValue(data, 'bay');
        const location: LocationEdit = {aisleId: aisleId, bay: bay};
        await editItemAisle(cookies, itemId, location);
    }
}