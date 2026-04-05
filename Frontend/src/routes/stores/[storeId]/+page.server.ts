import type {Actions, PageServerLoad} from "./$types";
import {getAllAisles, getAllStores} from "$lib/scripts/requests/get.js";
import {getValue, getValueNumber} from "$lib/scripts/requests/common.js";
import {addAisle} from "$lib/scripts/requests/post.js";
import {editAisleName, editAisleOrder} from "$lib/scripts/requests/patch.js";
import {deleteAisle} from "$lib/scripts/requests/delete.js";
import type Store from "$lib/scripts/classes/Store.ts";
import type Aisle from "$lib/scripts/classes/Aisle.ts";

let storeId: string = '';

export const load: PageServerLoad = async ({params, cookies}) => {
    storeId = params.storeId;
    
    const stores: Store[] = await getAllStores(cookies);
    const store: Store = stores.filter(s => s.storeId === params.storeId)[0];
    const aisles: Aisle[] = await getAllAisles(cookies, store.storeId);
    
    return {
        store: store,
        aisles: aisles
    }
}

export const actions: Actions = {
    addAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleName: string = await getValue(data, 'inputAdd');
        await addAisle(cookies, storeId, aisleName);
    },
    renameAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        const aisleName: string = await getValue(data, 'inputRename');
        await editAisleName(cookies, aisleId, aisleName);
    },
    deleteAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        await deleteAisle(cookies, aisleId);
    },
    reorderAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        const sortSortOrder: number = await getValueNumber(data, 'aisleSortOrder');
        await editAisleOrder(cookies, aisleId, sortSortOrder);
    }
}