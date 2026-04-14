import type {PageLoad} from './$types';
import type ItemDetails from "$lib/models/ItemDetails.ts";
import {get} from "$lib/functions/requests.js";

export const load: PageLoad = async ({fetch}) => {
    const ingredients: ItemDetails[] = await get<ItemDetails[]>('/api/items', fetch);
    return {
        ingredients: ingredients
    }
};