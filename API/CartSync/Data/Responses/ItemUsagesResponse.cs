using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemUsagesResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> Recipes { get; init; }
    
    public static Expression<Func<Item, ItemUsagesResponse>> FromEntity =>
        item => new ItemUsagesResponse
        {
            Id = item.ItemId,
            Name = item.ItemName,
            Recipes = item.RecipeSectionEntries
                .AsQueryable()
                .Select(r => r.RecipeSection.Recipe)
                .Distinct()
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .Select(RecipeMinimalResponse.FromEntity)
                .ToReadOnlyList()
        };
}