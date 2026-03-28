class PrepUsagesReport {
	constructor() {
		this.prepId = "";
		this.prepName = "";
		this.items = [];
        this.recipes = [];
	}
	
    prepId: string;
    prepName: string;
    items: ItemMinimal[];
    recipes: RecipeMinimal[];
    
	toMessage(): string {
        let message = "";
        message += `The item ${this.prepName} is current used by:\n`
        
        if (this.items.length > 0) {
            message += "Items:\n"
            
            let itemNames = this.items.map(i => i.itemName);
            message += (itemNames.length > 5 
                ? [itemNames.slice(0, 5),..."..."]
                : itemNames).join(", ");
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
        
        message += "Are you sure you want to delete this prep?";
        return message;
	}
}

export default PrepUsagesReport;