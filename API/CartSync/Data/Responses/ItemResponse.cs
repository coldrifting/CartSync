using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required Temp Temp { get; init; }
    public required UnitType DefaultUnitType { get; init; }
    public required bool UncapCartUnits { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ReadOnlyList<ItemAisleResponse> Locations { get; init; }
    
    public static Expression<Func<Item, ItemResponse>> FromEntity =>
        item => new ItemResponse
        {
            Id = item.ItemId,
            Name = item.ItemName,
            Temp = item.Temp,
            DefaultUnitType = item.DefaultUnitType,
            UncapCartUnits = item.UncapCartUnits,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(PrepResponse.FromEntity)
                .ToReadOnlyList(),
            Locations = item.ItemAisles
                .AsQueryable()
                .Select(ItemAisleResponse.FromEntity)
                .ToReadOnlyList()
        };
}