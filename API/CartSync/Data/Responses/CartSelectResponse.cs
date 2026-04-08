namespace CartSync.Data.Responses;

public record CartSelectResponse
{
    public CartSelectItemResponse[] Items { get; init; } = [];
    public CartSelectRecipeResponse[] Recipes { get; init; } = [];
}