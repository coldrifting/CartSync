class ItemUsagesReport {
	constructor() {
		this.itemId = "";
		this.itemName = "";
        this.recipes = [];
		this.preps = [];
	}
	
    itemId: string;
    itemName: string;
    recipes: RecipeMinimal[];
    preps: Prep[];
    
    toUsages(): Record<string, string[]> {
        return {
            'Recipes': this.recipes.map(r => r.recipeName),
            'Preps': this.preps.map(p => p.prepName)
        }
    }
}

export default ItemUsagesReport;