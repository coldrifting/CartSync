import type RecipeInstruction from "$lib/scripts/classes/RecipeInstruction.ts";
import type RecipeSection from "$lib/scripts/classes/RecipeSection.ts";

class RecipeDetails {
    recipeId: string = "";
    recipeName: string = "";
    url: string = "";
    isPinned: boolean = false;
    instructions: RecipeInstruction[] = [];
    sections: RecipeSection[] = [];
}

export default RecipeDetails;