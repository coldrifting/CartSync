import type Recipe from "$lib/scripts/classes/Recipe.ts";
import type Item from "$lib/scripts/classes/Item.ts";

class PrepUsagesReport {
    prepId: string = "";
    prepName: string = "";
    recipes: Recipe[] = [];
    items: Item[] = [];
    
    static getUsages(usages: PrepUsagesReport): Record<string, string[]> {
        return {
            'Recipes': usages.recipes.map(r => r.recipeName),
            'Items': usages.items.map(i => i.itemName)
        }
    }
}

export default PrepUsagesReport;