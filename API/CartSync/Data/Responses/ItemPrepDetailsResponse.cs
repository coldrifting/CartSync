using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemPrepDetailsResponse
{
    public required ItemMinimalResponse? Item { get; init; }
    public required ReadOnlyList<ItemPrepResponse> AllPreps { get; init; }
}