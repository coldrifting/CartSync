using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemMinimalResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required Temp Temp { get; init; }
    
    public static Expression<Func<Item, ItemMinimalResponse>> FromEntity =>
        item => new ItemMinimalResponse
        {
            Id = item.ItemId,
            Name = item.ItemName,
            Temp = item.Temp
        };
    
    public static ItemMinimalResponse FromNewEntity(Item item) => FromEntity.Compile()(item);
}