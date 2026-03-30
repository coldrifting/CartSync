import type {Actions, PageServerLoad} from './$types';
import ItemUsagesReport from "$lib/types/ItemUsagesReport.js";
import {fail} from "@sveltejs/kit";
import {getAllItems, getItemUsages} from "$lib/requests/get.js";
import {getValue} from "$lib/requests/requests.js";
import {addItem} from "$lib/requests/post.js";
import {deleteItem} from "$lib/requests/delete.js";
import {editItemName} from "$lib/requests/patch.js";

export const load: PageServerLoad = async ({cookies}) => {
    const ingredients: IngredientByStore[] = await getAllItems(cookies);
    return {
        ingredients: ingredients
    }
};

export const actions: Actions = {
    addItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemName = await getValue(data, 'inputAdd');
        await addItem(cookies, itemName)
    },
    renameItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const renameItemId = await getValue(data, 'id');
        const renameItemName = await getValue(data, 'inputRename');
        
        await editItemName(cookies, renameItemId, renameItemName);
    },
    tryDeleteItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const tryDeleteItemId = await getValue(data, 'id');
        const usages: ItemUsagesReport = await getItemUsages(cookies, tryDeleteItemId);
        if (usages && (usages.preps.length > 0 || usages.recipes.length > 0)) {
            const usageReport: Record<string, string[]> = usages.toUsages();
            return fail(409, usageReport);
        }
        
        await deleteItem(cookies, tryDeleteItemId);
    },
    deleteItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const deleteItemId = await getValue(data, 'id');
        await deleteItem(cookies, deleteItemId);
    }
};