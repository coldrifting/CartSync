import type Recipe from "$lib/models/Recipe.ts";
import type Item from "$lib/models/Item.ts";

class PrepUsagesReport {
    id: string = "";
    name: string = "";
    recipes: Recipe[] = [];
    items: Item[] = [];
    
    static getUsages(usages: PrepUsagesReport): Record<string, string[]> {
        return {
            'Recipes': usages.recipes.map(recipe => recipe.name),
            'Items': usages.items.map(item => item.name)
        }
    }
}

export default PrepUsagesReport;