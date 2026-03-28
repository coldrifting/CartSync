import type {Actions, PageServerLoad} from './$types';
import PrepUsagesReport from "$lib/types/PrepUsagesReport.js";
import {getAllPreps, getItem, getPrepUsages} from "$lib/requests/get.js";
import {getValue} from "$lib/requests/requests.js";
import {editItemPreps, editPrepName} from "$lib/requests/patch.js";
import {addPrep} from "$lib/requests/post.js";
import {deletePrep} from "$lib/requests/delete.js";
import {fail} from "@sveltejs/kit";

export const load: PageServerLoad = async ({params, cookies}) => {
    const item = await getItem(cookies, params.itemId);
    const preps = await getAllPreps(cookies);
    const selectedPreps: PrepSelect[] = preps.map(p => {
        return {
            prepId: p.prepId,
            prepName: p.prepName,
            isSelected: item.preps.map(p => p.prepId).includes(p.prepId)
        }
    });
    
    return {
        item: item,
        preps: selectedPreps,
    }
};

export const actions: Actions = {
    editPreps: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemId: string = await getValue(data, 'itemId');
        
        const preps: string[] = Array.from(data.entries().map((entry) => entry[0]).filter(i => i !== 'itemId'));

        console.log(itemId);
        console.log(preps);
        await editItemPreps(cookies, itemId, preps);
    },
    addPrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepName: string = await getValue(data, 'prepName');
        
        await addPrep(cookies, prepName);
    },
    renamePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepId: string = await getValue(data, 'id');
        const prepName: string = await getValue(data, 'prepName');
        
        await editPrepName(cookies, prepId, prepName);
    },
    tryDeletePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const tryDeletePrepId = await getValue(data, 'id');
        const usages: PrepUsagesReport = await getPrepUsages(cookies, tryDeletePrepId);
        if (usages && (usages.items.length > 0 || usages.recipes.length > 0)) {
            return fail(409, usages.toMessage());
        }
        
        await deletePrep(cookies, tryDeletePrepId);
    },
    deletePrep: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const prepId: string = await getValue(data, 'id');
        
        await deletePrep(cookies, prepId);
    }
}