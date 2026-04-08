import type Prep from "$lib/scripts/classes/Prep.ts";
import type RecipeDetails from "$lib/scripts/classes/RecipeDetails.ts";
import type itemDetails from "$lib/scripts/classes/ItemDetails.ts";
import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";
import type Item from "$lib/scripts/classes/Item.ts";

export class AllValidItems {
    sections: ValidSection[] = [];

    static fromData(recipe: RecipeDetails, items: itemDetails[]): AllValidItems {
        let sections: ValidSection[] = [];

        let recipeSections = [...recipe.sections, undefined];

        recipeSections.forEach((section, i) => {
            // Populate all combinations
            let validItems: ValidItem[] = [];
            items.forEach((item) => {
                let v = ValidItem.fromItem(item);
                validItems.push(v);
            })
            
            section?.entries.forEach((entry) => {
                // Remove 
                const itemIndex = validItems.map(itemAndPrep => itemAndPrep.id).indexOf(entry.item.id);
                if (itemIndex !== -1) {
                    const prepIndex = validItems[itemIndex].preps.map(prep => prep?.id).indexOf(entry.prep?.id);
                    if (prepIndex !== -1) {
                        validItems[itemIndex].preps.splice(prepIndex, 1);
                        if (validItems[itemIndex].preps.length === 0) {
                            validItems.splice(itemIndex, 1);
                        }
                    }
                }
            })

            sections[i] = {
                id: section?.id,
                name: section?.name,
                items: validItems
            };
        })

        return {
            sections: sections,
        } as AllValidItems;
    }
}

export class ValidSection {
    id?: string | undefined = "";
    name?: string | undefined = "";
    items: ValidItem[] = [];
}

export class ValidItem {
    id: string = "";
    name: string = "";
    defaultUnitType: string = "";
    preps: (Prep | null)[] = [];

    static fromItem(item: ItemDetails): ValidItem {
        return {
            id: item.id,
            name: item.name,
            defaultUnitType: item.defaultUnitType,
            preps: [null, ...item.preps]
        } as ValidItem;
    }
}