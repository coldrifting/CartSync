import type CartSelectItem from "$lib/models/CartSelectItem.ts";
import type CartSelectRecipe from "$lib/models/CartSelectRecipe.ts";
import type Recipe from "$lib/models/Recipe.ts";
import type ItemWithPreps from "$lib/models/ItemWithPreps.ts";

class CartSelect {
    recipes: CartSelectRecipe[] = [];
    items: CartSelectItem[] = [];
    remainingRecipes: Recipe[] = [];
    remainingItems: ItemWithPreps[] = [];
}

export default CartSelect;