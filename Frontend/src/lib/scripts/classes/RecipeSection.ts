import type RecipeEntry from "$lib/scripts/classes/RecipeEntry.ts";

class RecipeSection {
    id: string = "";
    name: string = "";
    sortOrder: number = 0;
    entries: RecipeEntry[] = [];
}

export default RecipeSection;