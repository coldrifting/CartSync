import type {Actions, PageServerLoad} from "./$types";
import {getAllAisles, getAllStores} from "$lib/requests/get.js";
import {getValue, getValueNumber} from "$lib/requests/requests.js";
import {addAisle} from "$lib/requests/post.js";
import {editAisleName, editAisleOrder} from "$lib/requests/patch.js";
import {deleteAisle} from "$lib/requests/delete.js";

export const load: PageServerLoad = async ({params, cookies}) => {
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
        const storeId: string = await getValue(data, 'storeId');
        const aisleName: string = await getValue(data, 'aisleName');
        await addAisle(cookies, storeId, aisleName);
    },
    renameAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'storeId');
        const aisleId: string = await getValue(data, 'aisleId');
        const aisleName: string = await getValue(data, 'aisleName');
        await editAisleName(cookies, storeId, aisleId, aisleName);
    },
    deleteAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'storeId');
        const aisleId: string = await getValue(data, 'aisleId');
        await deleteAisle(cookies, storeId, aisleId);
    },
    reorderAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'storeId');
        const aisleId: string = await getValue(data, 'aisleId');
        const sortSortOrder: number = await getValueNumber(data, 'aisleSortOrder');
        await editAisleOrder(cookies, storeId, aisleId, sortSortOrder);
    }
}