class PrepUsagesReport {
	constructor() {
		this.prepId = "";
		this.prepName = "";
        this.recipes = [];
		this.items = [];
	}
	
    prepId: string;
    prepName: string;
    recipes: RecipeMinimal[];
    items: ItemMinimal[];
    
    toUsages(): Record<string, string[]> {
        return {
            'Recipes': this.recipes.map(r => r.recipeName),
            'Items': this.items.map(i => i.itemName)
        }
    }
}

export default PrepUsagesReport;