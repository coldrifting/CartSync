import type RecipeStep from "$lib/scripts/classes/RecipeStep.ts";
import type RecipeSection from "$lib/scripts/classes/RecipeSection.ts";

class RecipeDetails {
    recipeId: string = "";
    recipeName: string = "";
    url: string = "";
    isPinned: boolean = false;
    steps: RecipeStep[] = [];
    sections: RecipeSection[] = [];
}

export default RecipeDetails;