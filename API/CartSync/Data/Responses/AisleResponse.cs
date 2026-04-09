using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record AisleResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
    
    public static Func<Aisle, AisleResponse> FromEntity =>
        aisle => new AisleResponse
        {
            Id = aisle.AisleId,
            Name = aisle.AisleName,
            SortOrder = aisle.SortOrder
        };

    public static AisleResponse FromNewEntity(Aisle aisle) => FromEntity(aisle);
}