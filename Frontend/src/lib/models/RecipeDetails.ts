import type RecipeStep from "$lib/models/RecipeStep.ts";
import type RecipeSection from "$lib/models/RecipeSection.ts";

class RecipeDetails {
    id: string = "";
    name: string = "";
    url: string = "";
    isPinned: boolean = false;
    steps: RecipeStep[] = [];
    sections: RecipeSection[] = [];
}

export default RecipeDetails;