import type {Cookies} from "@sveltejs/kit";
import {apiBaseUrl, checkForErrors, getToken} from "$lib/scripts/requests/common.js";
import type Amount from "$lib/scripts/classes/Amount.ts";

export async function editItemName(cookies: Cookies, itemId: string, itemName: string): Promise<void> {
    await patch(cookies, `/items/${itemId}/edit`, { "/ItemName": itemName });
}

export async function editItemTemp(cookies: Cookies, itemId: string, itemTemp: string): Promise<void> {
    await patch(cookies, `/items/${itemId}/edit`, { "/ItemTemp": itemTemp });
}

export async function editItemDefaultUnits(cookies: Cookies, itemId: string, defaultUnits: string): Promise<void> {
    await patch(cookies, `/items/${itemId}/edit`, { "/DefaultUnitType": defaultUnits });
}

export async function editItemPreps(cookies: Cookies, itemId: string, prepIds: string[]): Promise<void> {
    await patch(cookies, `/items/${itemId}/edit`, { "/PrepIds": prepIds });
}

export async function editPrepName(cookies: Cookies, prepId: string, prepName: string): Promise<void> {
    await patch(cookies, `/preps/${prepId}/edit`, { "/PrepName": prepName });
}

export async function editItemAisle(cookies: Cookies, itemId: string, location: LocationEdit | undefined): Promise<void> {
    await patch(cookies, `/items/${itemId}/edit`, { "/Location": location });
}

export async function editStoreName(cookies: Cookies, storeId: string, storeName: string): Promise<void> {
    await patch(cookies, `/stores/${storeId}/edit`, { "/StoreName": storeName });
}

export async function editAisleName(cookies: Cookies, storeId: string, aisleId: string, aisleName: string): Promise<void> {
    await patch(cookies, `/stores/${storeId}/aisles/${aisleId}/edit`, { "/AisleName": aisleName });
}

export async function editAisleOrder(cookies: Cookies, storeId: string, aisleId: string, sortOrder: number): Promise<void> {
    await patch(cookies, `/stores/${storeId}/aisles/${aisleId}/edit`, { "/SortOrder": sortOrder });
}

export async function editRecipeName(cookies: Cookies, recipeId: string, recipeName: string): Promise<void> {
    await patch(cookies, `/recipes/${recipeId}/edit`, { "/RecipeName": recipeName });
}

export async function editRecipeIsPinned(cookies: Cookies, recipeId: string, isPinned: boolean): Promise<void> {
    await patch(cookies, `/recipes/${recipeId}/edit`, { "/IsPinned": isPinned });
}

export async function editRecipeInstructionOrder(cookies: Cookies, recipeId: string, instructionId: string, sortOrder: number): Promise<void> {
    await patch(cookies, `/recipes/${recipeId}/instructions/${instructionId}/edit`, { "/SortOrder": sortOrder });
}

export async function editRecipeSectionName(cookies: Cookies, recipeId: string, recipeSectionId: string, sectionName: string): Promise<void> {
    await patch(cookies, `/recipes/${recipeId}/sections/${recipeSectionId}/edit`, { "/RecipeSectionName": sectionName });
}

export async function editRecipeEntry(cookies: Cookies, recipeId: string, recipeSectionId: string, recipeSectionEntryId: string, prepId: string | null, amount: Amount): Promise<void> {
    const url: string = `/recipes/${recipeId}/sections/${recipeSectionId}/entries/${recipeSectionEntryId}/edit`;
    await patch(cookies, url, {
        "/PrepId": prepId,
        "/Amount": amount
    });
}

export async function editRecipeUrl(cookies: Cookies, recipeId: string, url: string): Promise<void> {
    await patch(cookies, `/recipes/${recipeId}/edit`, { "/Url": url });
}
    
async function patch(cookies: Cookies, url: string, pathValuePairs: Record<string, any>): Promise<void> {
    const token = getToken(cookies);
            
    const patch = Object.entries(pathValuePairs).map(([key, value]) => {
        return {
            "op": "replace",
            "path": key,
            "value": value
        }
    })

    const response = await fetch(apiBaseUrl + url, {
        method: 'PATCH',
        body: JSON.stringify(patch),
        headers: {
            'Content-Type': 'application/json-patch+json',
            'Authorization': `Bearer ${token}`
        }
    });
    await checkForErrors(cookies, response);
}