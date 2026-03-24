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
		
	class IngredientByStore{
		itemId: string;
		itemName: string;
		itemTemp: string;
		defaultUnitType: string;
		preps: Prep[];
		location: Location | undefined;
	}
	
	class Ingredient{
		itemId: string;
		itemName: string;
		itemTemp: string;
		defaultUnitType: string;
		preps: Prep[];
		locations: Location[];
	}
	
	class Prep{
		prepId: string;
		prepName: string;
	}
	
	class Location{
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
}

export {};
