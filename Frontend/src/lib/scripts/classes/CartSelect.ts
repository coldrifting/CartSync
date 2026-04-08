import type CartSelectItem from "$lib/scripts/classes/CartSelectItem.ts";
import type CartSelectRecipe from "$lib/scripts/classes/CartSelectRecipe.ts";
import type Recipe from "$lib/scripts/classes/Recipe.ts";
import type ItemWithPreps from "$lib/scripts/classes/ItemWithPreps.ts";

class CartSelect {
    recipes: CartSelectRecipe[] = [];
    items: CartSelectItem[] = [];
    remainingRecipes: Recipe[] = [];
    remainingItems: ItemWithPreps[] = [];
}

export default CartSelect;