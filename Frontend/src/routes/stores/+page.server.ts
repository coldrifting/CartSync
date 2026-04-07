import type {Actions, PageServerLoad} from "./$types";
import {getAllStores} from "$lib/scripts/requests/get.js";
import {deleteStore} from "$lib/scripts/requests/delete.js";
import {getValue} from "$lib/scripts/requests/common.js";
import {addStore, setCurrentStore} from "$lib/scripts/requests/post.js";
import {editStoreName} from "$lib/scripts/requests/patch.js";

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
        await setCurrentStore(cookies, selectedStoreId);
    },
    addStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeName: string = await getValue(data, 'inputAdd');
        await addStore(cookies, storeName);
    },
    renameStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'id');
        const storeName: string = await getValue(data, 'inputRename');
        await editStoreName(cookies, storeId, storeName);
    },
    deleteStore: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const storeId: string = await getValue(data, 'id');
        await deleteStore(cookies, storeId);
    }
}