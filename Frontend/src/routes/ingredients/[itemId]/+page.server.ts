import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ params, cookies }) => {
    
    const token: string = cookies.get('token') ?? "";

    const url = `http://localhost:5164/api/items/${params.itemId}`;
    const response = await fetch(url, {
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        }})
    if (!response.ok) {
        throw new Error(`Response status: ${response.status}`);
    }
	
    const result: Ingredient = await response.json();
    
	return {
        ingredient: result
    }
};