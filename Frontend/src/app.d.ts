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
		actionRight: ButtonAction | undefined;
	}
	
	interface HeaderAction {
		label: string;
		icon: string;
		action: () => void;
		color?: string;
		hideFromMobile?: boolean;
		hideFromDesktop?: boolean;
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
	
	class ButtonAction {
		label: string;
		icon: string
		color: string;
		action: () => void;
	}
}

export {};
