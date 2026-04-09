import type {Actions, PageServerLoad} from "./$types";
import {getAllStores} from "$lib/requests/get.js";
import {deleteStore} from "$lib/requests/delete.js";
import {getValue} from "$lib/requests/common.js";
import {addStore, setCurrentStore} from "$lib/requests/post.js";
import {editStoreName} from "$lib/requests/patch.js";
import ErrorCustom from "$lib/models/ErrorCustom.js";

export const load: PageServerLoad = async ({cookies}) => {
    const stores = await getAllStores(cookies);
    return {
        selectedStore: stores.filter(store => store.isSelected)[0],
        stores: stores
    }
}

export const actions: Actions = {
    selectStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const selectedStoreId: string = await getValue(data, 'selectedStoreId');
        try {
            await setCurrentStore(cookies, selectedStoreId);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    addStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeName: string = await getValue(data, 'inputAdd');
        try {
            await addStore(cookies, storeName);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    renameStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'id');
        const storeName: string = await getValue(data, 'inputRename');
        try {
            await editStoreName(cookies, storeId, storeName);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    },
    deleteStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'id');
        try {
            await deleteStore(cookies, storeId);
        }
        catch (error) {
            error instanceof ErrorCustom 
                ? console.error(error.error) 
                : console.error(error);
        }
    }
}