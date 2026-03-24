import type {Actions, PageServerLoad} from './$types';
import ErrorResponse from "$lib/types/ErrorResponse.js";
import ItemUsagesReport from "$lib/types/ItemUsagesReport.js";
import {fail} from "@sveltejs/kit";

export const load: PageServerLoad = async ({ cookies }) => {
    
    const token: string = cookies.get('token') ?? "";
    
    const url = 'http://localhost:5164/api/items';
    const response = await fetch(url, {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
    }
    
    const ingredients: IngredientByStore[] = await response.json()
    
	return {
        ingredients: ingredients
    }
};

export const actions: Actions = {
  addIngredient: async ({ request, cookies }) => {
    const formData = await request.formData();
    const itemName = formData.get('itemName')
    if (!itemName || itemName === '') {
        throw new Error(`Invalid New Item Name: ${itemName}`);
    }
      
    const token: string = cookies.get('token') ?? "";
    const url: string = "http://localhost:5164/api/items/add";
    const response = await fetch(url, {
        method: 'POST',
        body: JSON.stringify({ itemName }),
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
    }
  },
  tryDelete: async ({ request, cookies }) => {
    const formData = await request.formData();
    const itemId = formData.get('id')
    if (!itemId || itemId === '') {
        throw new Error(`Invalid Item Id: ${itemId}`);
    }
    
    const token: string = cookies.get('token') ?? "";
    const url: string = `http://localhost:5164/api/items/${itemId}/usages`;
    const response = await fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
    const usages: ItemUsagesReport = Object.assign(new ItemUsagesReport(), await response.json());
    if (usages && (usages.preps.length > 0 || usages.recipes.length > 0)) {
        return fail(409, usages.toMessage())
    }
    
    const deleteUrl: string = `http://localhost:5164/api/items/${itemId}/delete`;
    const deleteResponse = await fetch(deleteUrl, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!deleteResponse.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await deleteResponse.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
  },
  delete: async ({ request, cookies }) => {
    const formData = await request.formData();
    const itemId = formData.get('id')
    if (!itemId || itemId === '') {
        throw new Error(`Invalid Item Id: ${itemId}`);
    }
    
    const token: string = cookies.get('token') ?? "";
    const url: string = `http://localhost:5164/api/items/${itemId}/delete`;
    const response = await fetch(url, {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
  },
  rename: async ({ request, cookies }) => {
    const formData = await request.formData();
    const itemId = formData.get('id')
    if (!itemId || itemId === '') {
        throw new Error(`Invalid Item Id: ${itemId}`);
    }
    const newName = formData.get('newName')
    if (!newName || newName === '') {
        throw new Error(`Invalid Item Name: ${newName}`);
    }
    
    const token: string = cookies.get('token') ?? "";
    const url: string = `http://localhost:5164/api/items/${itemId}/edit`;
    const response = await fetch(url, {
        method: 'PATCH',
        body: JSON.stringify([{ 
            "op": "replace", 
            "path": "/ItemName", 
            "value": newName 
        }]),
        headers: {
            'Content-Type': 'application/json-patch+json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        const error: ErrorResponse = Object.assign(new ErrorResponse(), await response.json());
        const errorMsg = error.getErrorMsg();
        throw new Error(errorMsg);
    }
  }
};