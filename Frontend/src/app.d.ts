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
		isDestructive: boolean;
		action: (val: string, data: string | null) => void;
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
}

export {};
