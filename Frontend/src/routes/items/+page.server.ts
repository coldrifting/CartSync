import type {Actions, PageServerLoad} from './$types';
import {fail} from "@sveltejs/kit";
import {getAllItems, getItemUsages} from "$lib/requests/get.js";
import {getValue} from "$lib/requests/common.js";
import {addItem} from "$lib/requests/post.js";
import {deleteItem} from "$lib/requests/delete.js";
import {editItemName} from "$lib/requests/patch.js";
import ItemUsagesReport from "$lib/models/ItemUsagesReport.js";
import type ItemDetails from "$lib/models/ItemDetails.ts";
import ErrorCustom from "$lib/models/ErrorCustom.js";

export const load: PageServerLoad = async ({cookies}) => {
    const ingredients: ItemDetails[] = await getAllItems(cookies);
    return {
        ingredients: ingredients
    }
};

export const actions: Actions = {
    addItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemName = await getValue(data, 'inputAdd');
        try {
            await addItem(cookies, itemName)
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    renameItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const renameItemId = await getValue(data, 'id');
        const renameItemName = await getValue(data, 'inputRename');
        try {
            await editItemName(cookies, renameItemId, renameItemName);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    tryDeleteItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const tryDeleteItemId = await getValue(data, 'id');
        try {
            const usages: ItemUsagesReport = await getItemUsages(cookies, tryDeleteItemId);
            if (usages && usages.recipes.length > 0) {
                const usageReport: Record<string, string[]> = ItemUsagesReport.getUsages(usages);
                return fail(409, usageReport);
            }

            await deleteItem(cookies, tryDeleteItemId);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    },
    deleteItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const deleteItemId = await getValue(data, 'id');
        try {
            await deleteItem(cookies, deleteItemId);
        } catch (error) {
            error instanceof ErrorCustom
                ? console.error(error.error)
                : console.error(error);
        }
    }
};