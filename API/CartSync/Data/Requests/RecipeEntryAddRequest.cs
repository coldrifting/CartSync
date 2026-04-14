using CartSync.Objects;

namespace CartSync.Data.Requests;

public record RecipeEntryAddRequest
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}