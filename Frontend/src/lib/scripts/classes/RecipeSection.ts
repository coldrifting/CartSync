import type RecipeSectionEntry from "$lib/scripts/classes/RecipeSectionEntry.ts";

class RecipeSection {
    recipeSectionId: string = "";
    recipeSectionName: string = "";
    sortOrder: number = 0;
    entries: RecipeSectionEntry[] = [];
}

export default RecipeSection;