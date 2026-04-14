import type {PageLoad} from './$types';
import type ItemDetails from "$lib/models/ItemDetails.ts";
import type Prep from "$lib/models/Prep.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch, params}) => {
    const [item, preps] = await Promise.all([
        await get<ItemDetails>(`/api/items/${params.itemId}`, fetch),
        await get<Prep[]>('/api/preps', fetch)
    ]);
    
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