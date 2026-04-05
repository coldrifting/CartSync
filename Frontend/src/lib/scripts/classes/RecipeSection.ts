import type RecipeEntry from "$lib/scripts/classes/RecipeEntry.ts";

class RecipeSection {
    recipeSectionId: string = "";
    recipeSectionName: string = "";
    sortOrder: number = 0;
    entries: RecipeEntry[] = [];
}

export default RecipeSection;