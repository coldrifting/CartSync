using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record PrepUsagesResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required ReadOnlyList<ItemMinimalResponse> Items { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> Recipes { get; init; }
    
    public static Expression<Func<Prep, PrepUsagesResponse>> FromEntity =>
        prep => new PrepUsagesResponse
        {
            Id = prep.PrepId,
            Name = prep.PrepName,
            Items = prep.Items
                .AsQueryable()
                .OrderBy(i => i.ItemName)
                .ThenBy(i => i.ItemId)
                .Select(ItemMinimalResponse.FromEntity)
                .ToReadOnlyList(),
            Recipes = prep.RecipeSectionEntries
                .AsQueryable()
                .Select(r => r.RecipeSection.Recipe)
                .Distinct()
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .Select(RecipeMinimalResponse.FromEntity)
                .ToReadOnlyList()
        };
}