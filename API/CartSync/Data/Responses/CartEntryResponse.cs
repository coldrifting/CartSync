using CartSync.Objects;

namespace CartSync.Data.Responses;

public record CartEntryResponse
{
    public required Ulid StoreId { get; init; }
    public required string StoreName { get; init; }
    public required ReadOnlyList<CartEntryAisleResponse> Aisles { get; init; }
}