import type Recipe from "$lib/models/Recipe.ts";

class ItemUsagesReport {
    id: string = "";
    name: string = "";
    recipes: Recipe[] = [];
    
    static getUsages(usages: ItemUsagesReport): Record<string, string[]> {
        return {
            'Recipes': usages.recipes.map(recipe => recipe.name),
        }
    }
}

export default ItemUsagesReport;