using System.Collections.Immutable;
using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record RecipeSectionResponse
{
    public required Ulid? Id { get; init; }
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
    public required ReadOnlyList<RecipeEntryResponse> Entries { get; init; }
    
    public static Expression<Func<RecipeSection, RecipeSectionResponse>> FromEntity =>
        section => new RecipeSectionResponse
        {
            Id = section.RecipeSectionId,
            Name = section.RecipeSectionName,
            SortOrder = section.SortOrder,
            Entries = section.Entries
                .AsQueryable()
                .OrderBy(entry => entry.Item.Temp)
                .ThenBy(entry => entry.Item.ItemName)
                .ThenBy(entry => entry.Item.ItemId)
                .ThenBy(entry => entry.Prep != null ? entry.Prep.PrepName : "$None" )
                .Select(RecipeEntryResponse.FromEntity)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public static RecipeSectionResponse FromNewEntity(RecipeSection section) => FromEntity.Compile()(section);
}