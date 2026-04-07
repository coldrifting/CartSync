import type Prep from "$lib/scripts/classes/Prep.ts";
import type Recipe from "$lib/scripts/classes/Recipe.ts";

class ItemUsagesReport {
    id: string = "";
    name: string = "";
    recipes: Recipe[] = [];
    preps: Prep[] = [];
    
    static getUsages(usages: ItemUsagesReport): Record<string, string[]> {
        return {
            'Recipes': usages.recipes.map(recipe => recipe.recipeName),
            'Preps': usages.preps.map(prep => prep.prepName)
        }
    }
}

export default ItemUsagesReport;