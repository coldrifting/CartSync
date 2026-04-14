using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemAisleResponse
{
    public required Ulid AisleId { get; init; }
    public required Ulid StoreId { get; init; }
    public required string AisleName { get; init; }
    public required Bay Bay { get; init; }
    public required int SortOrder { get; init; }
    
    public static Expression<Func<ItemAisle, ItemAisleResponse>> FromEntity =>
        itemAisle => new ItemAisleResponse
        {
            AisleId = itemAisle.AisleId,
            StoreId = itemAisle.StoreId,
            AisleName = itemAisle.Aisle.AisleName,
            SortOrder = itemAisle.Aisle.SortOrder,
            Bay = itemAisle.Bay
        };
}