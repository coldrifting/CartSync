import type {Actions, PageServerLoad} from './$types';

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
};