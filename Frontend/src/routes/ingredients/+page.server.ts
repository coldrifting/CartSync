import type {Actions, PageServerLoad} from './$types';
import ItemUsagesReport from "$lib/types/ItemUsagesReport.js";
import {fail} from "@sveltejs/kit";
import {getAllItemsByStore, getItemUsages} from "$lib/requests/get.js";
import {getValue} from "$lib/requests/requests.js";
import {addItem} from "$lib/requests/post.js";
import {deleteItem} from "$lib/requests/delete.js";
import {editItemName} from "$lib/requests/patch.js";

export const load: PageServerLoad = async ({cookies}) => {
    const ingredients = await getAllItemsByStore(cookies);
    return {
        ingredients: ingredients
    }
};

export const actions: Actions = {
    addIngredient: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemName = await getValue(data, 'itemName');
        await addItem(cookies, itemName)
    },
    tryDelete: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const tryDeleteItemId = await getValue(data, 'id');
        const usages: ItemUsagesReport = await getItemUsages(cookies, tryDeleteItemId);
        if (usages && (usages.preps.length > 0 || usages.recipes.length > 0)) {
            return fail(409, usages.toMessage());
        }
        
        await deleteItem(cookies, tryDeleteItemId);
    },
    delete: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const deleteItemId = await getValue(data, 'id');
        await deleteItem(cookies, deleteItemId);
    },
    rename: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const renameItemId = await getValue(data, 'id');
        const renameItemName = await getValue(data, 'newName');
        
        await editItemName(cookies, renameItemId, renameItemName);
    }
};