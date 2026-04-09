import type {Actions, PageServerLoad} from "./$types";
import {redirect} from "@sveltejs/kit";
import {getValue, getValueBoolean, getValueNumber, getValueOrNull} from "$lib/scripts/requests/common.js";
import {getCartSelection} from "$lib/scripts/requests/get.js";
import {editCartItem, editCartRecipe} from "$lib/scripts/requests/put.js";
import type CartSelect from "$lib/scripts/classes/CartSelect.ts";
import Fraction from "$lib/scripts/classes/Fraction.js";
import type Amount from "$lib/scripts/classes/Amount.ts";
import {deleteCartItem, deleteCartRecipe} from "$lib/scripts/requests/delete.js";
import {generateCart} from "$lib/scripts/requests/post.js";

export const load: PageServerLoad = async ({cookies}) => {
    const cartSelection: CartSelect = await getCartSelection(cookies);
    
    return {
        items: cartSelection.items,
        recipes: cartSelection.recipes,
        remainingRecipes: cartSelection.remainingRecipes,
        remainingItems: cartSelection.remainingItems,
    }
}

export const actions: Actions = {
    generateCart: async ({cookies}) => {
        await generateCart(cookies);
        redirect(303, '/cart/list')
    },
    addCartItemOrRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const isRecipeType: boolean = await getValueBoolean(data, 'isRecipeType');
        if (isRecipeType) {
            const recipeId: string = await getValue(data, 'recipeId');
            const recipeQuantity: number = await getValueNumber(data, 'recipeQuantity');
            
            await editCartRecipe(cookies, recipeId, recipeQuantity);
        }
        else {
            const itemId: string = await getValue(data, 'itemId');
            const prepId: string | null = await getValueOrNull(data, "prepId");
            
            const fractionString: string = await getValue(data, "fraction");
            const fraction: Fraction = Fraction.fromNumberString(fractionString);
            const unitType: string = await getValue(data, "unitType");
            const amount: Amount = {fraction: fraction, unitType: unitType} as Amount;
            
            await editCartItem(cookies, itemId, prepId, amount);
        }
    },
    editCartRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        
        const recipeId: string = await getValue(data, 'recipeId');
        const recipeQuantity: number = await getValueNumber(data, 'recipeQuantity');
        
        await editCartRecipe(cookies, recipeId, recipeQuantity);
    },
    editCartItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        
        const itemId: string = await getValue(data, 'itemId');
        const prepId: string | null = await getValueOrNull(data, "prepId");
        
        const fractionString: string = await getValue(data, "fraction");
        const fraction: Fraction = Fraction.fromNumberString(fractionString);
        const unitType: string = await getValue(data, "unitType");
        const amount: Amount = {fraction: fraction, unitType: unitType} as Amount;
        
        await editCartItem(cookies, itemId, prepId, amount);
    },
    removeCartRecipe: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const recipeId: string = await getValue(data, 'recipeId');
        
        await deleteCartRecipe(cookies, recipeId);
    },
    removeCartItem: async ({request, cookies}) => {
        const data: FormData = await request.formData();
        const itemId: string = await getValue(data, 'itemId');
        const prepId: string | null = await getValueOrNull(data, "prepId");
        
        await deleteCartItem(cookies, itemId, prepId);
    }
}