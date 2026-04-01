import type {Actions, PageServerLoad} from './$types';
import {fail} from "@sveltejs/kit";
import {getAllPreps, getItem, getPrepUsages} from "$lib/scripts/requests/get.js";
import {getValue, getValueArray} from "$lib/scripts/requests/common.js";
import {addPrep} from "$lib/scripts/requests/post.js";
import {editItemPreps, editPrepName} from "$lib/scripts/requests/patch.js";
import {deletePrep} from "$lib/scripts/requests/delete.js";
import PrepUsagesReport from "$lib/scripts/classes/PrepUsagesReport.js";
import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";
import type Prep from "$lib/scripts/classes/Prep.ts";

export const load: PageServerLoad = async ({params, cookies}) => {
    const item: ItemDetails = await getItem(cookies, params.itemId);
    const preps: Prep[] = await getAllPreps(cookies);
    const selectedPreps: PrepSelect[] = preps.map(p => {
        return {
            prepId: p.prepId,
            prepName: p.prepName,
            isSelected: item.preps.map(p => p.prepId).includes(p.prepId)
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