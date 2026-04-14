using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record RecipeEntryResponse
{
    public required Ulid Id { get; init; }
    public required ItemMinimalResponse Item { get; init; }
    public required PrepResponse? Prep { get; init; }
    public required Amount Amount { get; init; }
    
    public static Expression<Func<RecipeEntry, RecipeEntryResponse>> FromEntity =>
        entry => new RecipeEntryResponse
        {
            Id = entry.RecipeEntryId,
            Item = ItemMinimalResponse.FromEntity.Compile()(entry.Item),
            Prep = entry.Prep != null 
                ? PrepResponse.FromEntity.Compile()(entry.Prep) 
                : null,
            Amount = entry.Amount
        };

    public static RecipeEntryResponse FromNewEntity(RecipeEntry entry) => FromEntity.Compile()(entry);
}