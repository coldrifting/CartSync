// See https://svelte.dev/docs/kit/types#app.d.ts
// for information about these interfaces
declare global {
	namespace App {
		// interface Error {}
		// interface Locals {}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
		
	interface SortableItem {
		id: string;
		name: string;
        subtitle: string;
	}
	
	interface HeaderAction {
		label: string;
		icon: string;
		action: () => void;
	}
	
	class Store {
		storeId: string;
		storeName: string;
		isSelected: boolean;
	}
	
	class Aisle {
		aisleId: string;
		storeId: string;
		aisleName: string;
		sortOrder: number;
	}
	
	class AisleDragAndDrop {
		id: string;
		label: string;
	}
	
	class IngredientByStore {
		itemId: string;
		itemName: string;
		itemTemp: string;
		defaultUnitType: string;
		preps: Prep[];
		location: Location | undefined;
	}
	
	class Ingredient {
		itemId: string;
		itemName: string;
		itemTemp: string;
		defaultUnitType: string;
		preps: Prep[];
		locations: Location[];
	}
	
	class Prep {
		prepId: string;
		prepName: string;
	}
	
	class PrepSelect {
		prepId: string;
		prepName: string;
		isSelected: boolean;
	}
	
	class LocationEdit {
		aisleId: string;
		bay: string;
	}
	
	class Location {
		aisleId: string;
		aisleName: string;
		bay: string;
		storeId: string;
		sortOrder: number;
	}
	
	class NavInfo {
		url: string;
		name: string;
		icon: string;
	}
	
	class ContextAction {
		label: string;
		action: (id: string, value: string | undefined) => void;
	}
	
	class ItemMinimal {
		itemId: string;
		itemName: string;
		itemTemp: string;
	}
	
	class RecipeMinimal {
		recipeId: string;
		recipeName: string;
		url: string;
		isPinned: boolean;
	}
	
	class Fraction {
		num: number;
		dem: number;
	}
	
	class Amount {
		fraction: Fraction;
		unitType: string;
	}
	
	class RecipeInstruction {
		recipeInstructionId: string;
		recipeInstructionContent: string;
		isImage: boolean;
		sortOrder: number;
	}
	
	class RecipeSectionEntry {
		recipeSectionEntryId: string;
		recipeSectionId: string;
		sortOrder: number;
		item: ItemMinimal;
		prep: Prep | null;
		amount: Amount;
	}
	
	class RecipeSection {
		recipeSectionId: string;
		recipeSectionName: string;
		sortOrder: number;
		entries: RecipeSectionEntry[];
	}
	
	class Recipe {
		recipeId: string;
		recipeName: string;
		url: string;
		isPinned: boolean;
		instructions: RecipeInstruction[];
		sections: RecipeSection[];
	}
}

export {};
