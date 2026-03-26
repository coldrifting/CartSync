import type {Actions, PageServerLoad} from './$types';
import {getAllAisles, getAllPreps, getAllStores, getItem} from "$lib/requests/get.js";
import {getValue} from "$lib/requests/requests.js";
import {editItemDefaultUnits, editItemTemp} from "$lib/requests/patch.js";
import type {Option} from "svelte-multiselect";

export const load: PageServerLoad = async ({params, cookies}) => {
    
    const stores = await getAllStores(cookies);
    const selectedStore = stores.filter(s => s.isSelected)[0];
    
    const [aisles, preps, item] = await Promise.all([
        getAllAisles(cookies, selectedStore.storeId),
        getAllPreps(cookies),
        getItem(cookies, params.itemId)
    ]);
    
    const allPreps: Option[] = preps.map(prep => ({label: prep.prepName, value: prep.prepId}));
    const selectedPreps: Option[] = item.preps.map(prep => ({label: prep.prepName, value: prep.prepId}));

    return {
        stores: stores,
        selectedStore: selectedStore,
        aisles: aisles,
        preps: preps,
        allPreps: allPreps,
        selectedPreps: selectedPreps,
        ingredient: item
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
    }
}