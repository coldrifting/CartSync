class ItemUsagesReport {
	constructor() {
		this.itemId = "";
		this.itemName = "";
		this.preps = [];
        this.recipes = [];
	}
	
    itemId: string;
    itemName: string;
    preps: Prep[];
    recipes: RecipeMinimal[];
    
	toMessage(): string {
        let message = "";
        message += `The item ${this.itemName} is current used by:\n`
        
        if (this.preps.length > 0) {
            message += "Preps:\n"
            
            let prepNames = this.preps.map(p => p.prepName);
            message += (prepNames.length > 5 
                ? [prepNames.slice(0, 5),..."..."]
                : prepNames).join(", ");
            message += "\n";
        }
        
        if (this.recipes.length > 0) {
            message += "Recipes:\n"
            
            let recipeNames = this.recipes.map(r => r.recipeName);
            message += (recipeNames.length > 5 
                ? [recipeNames.slice(0, 5),..."..."]
                : recipeNames).join(", ");
            message += "\n";
        }
        
        message += "Are you sure you want to delete this item?";
        return message;
	}
}

export default ItemUsagesReport;