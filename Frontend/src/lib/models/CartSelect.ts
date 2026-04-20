import type CartSelectItem from "$lib/models/CartSelectItem.ts";
import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
import type Recipe from "$lib/models/Recipe.ts";
import type ItemWithPreps from "$lib/models/ItemWithPreps.ts";

class CartSelect {
    cartLastGeneratedTime: number = 0;
    cartSelectionLastUpdatedTime: number = 0;
    recipes: CartSelectRecipe[] = [];
    items: CartSelectItem[] = [];
    remainingRecipes: Recipe[] = [];
    remainingItems: ItemWithPreps[] = [];
}

export default CartSelect;