import type {Actions, PageServerLoad} from "./$types";
import {getAllAisles, getAllStores} from "$lib/requests/get.js";
import {getValue, getValueNumber} from "$lib/requests/common.js";
import {addAisle} from "$lib/requests/post.js";
import {editAisleName, editAisleOrder} from "$lib/requests/patch.js";
import {deleteAisle} from "$lib/requests/delete.js";
import type Store from "$lib/models/Store.ts";
import type Aisle from "$lib/models/Aisle.ts";
import ErrorCustom from "$lib/models/ErrorCustom.js";

let storeId: string = '';

export const load: PageServerLoad = async ({params, cookies}) => {
    storeId = params.storeId;
    
    const stores: Store[] = await getAllStores(cookies);
    const store: Store = stores.filter(store => store.id === params.storeId)[0];
    const aisles: Aisle[] = await getAllAisles(cookies, store.id);
    
    return {
        store: store,
        aisles: aisles
    }
}

export const actions: Actions = {
    addAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleName: string = await getValue(data, 'inputAdd');
        try {
            await addAisle(cookies, storeId, aisleName);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    renameAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        const aisleName: string = await getValue(data, 'inputRename');
        try {
            await editAisleName(cookies, aisleId, aisleName);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    deleteAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        try {
            await deleteAisle(cookies, aisleId);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    reorderAisle: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const aisleId: string = await getValue(data, 'id');
        const sortSortOrder: number = await getValueNumber(data, 'aisleSortOrder');
        try {
            await editAisleOrder(cookies, aisleId, sortSortOrder);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    }
}