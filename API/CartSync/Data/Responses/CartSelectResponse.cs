using CartSync.Objects;

namespace CartSync.Data.Responses;

public record CartSelectResponse
{
    public required ReadOnlyList<CartSelectItemResponse> Items { get; init; }
    public required ReadOnlyList<CartSelectRecipeResponse> Recipes { get; init; }
    
    public required ReadOnlyList<ItemWithPrepsResponse> RemainingItems { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> RemainingRecipes { get; init; }
}