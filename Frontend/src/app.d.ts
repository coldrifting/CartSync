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
		isContent: boolean | undefined;
		isImage: boolean | undefined;
	}
	
	interface HeaderAction {
		label: string;
		icon: string;
		action: () => void;
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
	
	class NavInfo {
		url: string;
		name: string;
		icon: string;
	}
	
	class ContextAction {
		label: string;
		action: (id: string, value: string | undefined) => void;
	}
}

export {};
