namespace CartSync.Models.Seeding;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<RecipeSection> RecipeSections =>
    [
        new() { RecipeSectionId = Ulid.Parse("01KJA5G3X8HR8YEY9EAQXDQHS6"), RecipeId = Recipes[0].RecipeId, SortOrder = 0, RecipeSectionName = "Main" },
        new() { RecipeSectionId = Ulid.Parse("01KJA5G3X8NYZG081G3WMAEGX8"), RecipeId = Recipes[1].RecipeId, SortOrder = 0, RecipeSectionName = "Main" },
        new() { RecipeSectionId = Ulid.Parse("01KJA5G3X8558WCDNN06Q475BQ"), RecipeId = Recipes[2].RecipeId, SortOrder = 0, RecipeSectionName = "Main" },
        new() { RecipeSectionId = Ulid.Parse("01KJA5G3X8CZCPT74P9CS96CNP"), RecipeId = Recipes[3].RecipeId, SortOrder = 0, RecipeSectionName = "Main" }
    ];
}