import type Prep from "$lib/scripts/classes/Prep.ts";
import type Recipe from "$lib/scripts/classes/Recipe.ts";

class ItemUsagesReport {
    itemId: string = "";
    itemName: string = "";
    recipes: Recipe[] = [];
    preps: Prep[] = [];
    
    static getUsages(usages: ItemUsagesReport): Record<string, string[]> {
        return {
            'Recipes': usages.recipes.map(r => r.recipeName),
            'Preps': usages.preps.map(p => p.prepName)
        }
    }
}

export default ItemUsagesReport;