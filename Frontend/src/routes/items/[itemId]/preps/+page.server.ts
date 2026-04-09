import type {Actions, PageServerLoad} from './$types';
import {fail} from "@sveltejs/kit";
import {getAllPreps, getItem, getPrepUsages} from "$lib/requests/get.js";
import {getValue, getValueArray} from "$lib/requests/common.js";
import {addPrep} from "$lib/requests/post.js";
import {editItemPreps, editPrepName} from "$lib/requests/patch.js";
import {deletePrep} from "$lib/requests/delete.js";
import PrepUsagesReport from "$lib/models/PrepUsagesReport.js";
import type ItemDetails from "$lib/models/ItemDetails.ts";
import type Prep from "$lib/models/Prep.ts";

export const load: PageServerLoad = async ({params, cookies}) => {
    const item: ItemDetails = await getItem(cookies, params.itemId);
    const preps: Prep[] = await getAllPreps(cookies);
    const selectedPreps: PrepSelect[] = preps.map(prep => {
        return {
            prepId: prep.id,
            prepName: prep.name,
            isSelected: item.preps.map(itemPrep => itemPrep.id).includes(prep.id)
        }
    });
    
    return {
        item: item,
        preps: selectedPreps
    }
};

export const actions: Actions = {
    addPrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepName: string = await getValue(data, 'inputAdd');
        
        await addPrep(cookies, prepName);
    },
    renamePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepId: string = await getValue(data, 'id');
        const prepName: string = await getValue(data, 'inputRename');
        
        await editPrepName(cookies, prepId, prepName);
    },
    tryDeletePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const tryDeletePrepId = await getValue(data, 'id');
        const usages: PrepUsagesReport = await getPrepUsages(cookies, tryDeletePrepId);
        if (usages && (usages.items.length > 0 || usages.recipes.length > 0)) {
            const usageReport: Record<string, string[]> = PrepUsagesReport.getUsages(usages);
            return fail(409, usageReport);
        }
        
        await deletePrep(cookies, tryDeletePrepId);
    },
    deletePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepId: string = await getValue(data, 'id');
        
        await deletePrep(cookies, prepId);
    },
    editPreps: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemId: string = await getValue(data, 'itemId');
        const selectedPrepIds: string[] = await getValueArray(data, 'selectedPrepIds');
        
        await editItemPreps(cookies, itemId, selectedPrepIds);
    }
}