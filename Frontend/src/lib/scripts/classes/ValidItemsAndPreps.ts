import type Prep from "$lib/scripts/classes/Prep.ts";
import type RecipeDetails from "$lib/scripts/classes/RecipeDetails.ts";
import type itemDetails from "$lib/scripts/classes/ItemDetails.ts";
import type ItemDetails from "$lib/scripts/classes/ItemDetails.ts";

export class ItemsAndPrepsBySection {
    sections: ItemsAndPrepsSection[] = [];

    static fromData(recipe: RecipeDetails, items: itemDetails[]): ItemsAndPrepsBySection {
        let sections: ItemsAndPrepsSection[] = [];

        let recipeSections = [...recipe.sections, undefined];

        recipeSections.forEach((section, i) => {
            // Populate all combinations
            let validItems: ItemAndPreps[] = [];
            items.forEach((item) => {
                let ValidItem = ItemAndPreps.fromItem(item);
                validItems.push(ValidItem);
            })
            
            section?.entries.forEach((entry) => {
                // Remove 
                const itemIndex = validItems.map(i => i.itemId).indexOf(entry.item.itemId);
                if (itemIndex !== -1) {
                    const prepIndex = validItems[itemIndex].preps.map(p => p?.prepId).indexOf(entry.prep?.prepId);
                    if (prepIndex !== -1) {
                        validItems[itemIndex].preps.splice(prepIndex, 1);
                        if (validItems[itemIndex].preps.length === 0) {
                            validItems.splice(itemIndex, 1);
                        }
                    }
                }
            })

            sections[i] = {
                sectionId: section?.recipeSectionId,
                sectionName: section?.recipeSectionName,
                validItems: validItems
            };
        })

        return {
            sections: sections,
        } as ItemsAndPrepsBySection;
    }
}

export class ItemsAndPrepsSection {
    sectionId?: string | undefined = "";
    sectionName?: string | undefined = "";
    validItems: ItemAndPreps[] = [];
}

export class ItemAndPreps {
    itemId: string = "";
    itemName: string = "";
    defaultUnitType: string = "";
    preps: (Prep | null)[] = [];

    static fromItem(item: ItemDetails): ItemAndPreps {
        return {
            itemId: item.itemId,
            itemName: item.itemName,
            defaultUnitType: item.defaultUnitType,
            preps: [null, ...item.preps]
        } as ItemAndPreps;
    }
}