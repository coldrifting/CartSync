namespace CartSyncBackend.Models.Seeding;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<Recipe> Recipes =>
    [ 
        new() { RecipeId = Ulid.Parse("01KJ9PVFD1TYQQECV2GSSH6BHH"), RecipeName = "Pasta Alfredo", Url = "https://www.budgetbytes.com/lighter-spinach-alfredo-pasta/", IsPinned = false, CartAmount = 0 }, 
        new() { RecipeId = Ulid.Parse("01KJ9PVFD2VQAAP34S5HW2JPY8"), RecipeName = "Green Chile Mac & Cheese", Url = "https://www.budgetbytes.com/green-chile-mac-and-cheese/", IsPinned = true, CartAmount = 2 }, 
        new() { RecipeId = Ulid.Parse("01KJ9PVFD2JW12Q4Q5EVDCJEDB"), RecipeName = "Hummus Pita Bread", Url = "https://www.budgetbytes.com/loaded-hummus-pitas/", IsPinned = false, CartAmount = 0 }, 
        new() { RecipeId = Ulid.Parse("01KJ9PVFD2ZQP78817A8RN41MZ"), RecipeName = "Stuffed Poblano Peppers", Url = "https://www.loveandlemons.com/stuffed-poblano-peppers/", IsPinned = false, CartAmount = 1 }
    ];
}