using CartSync.Objects;

namespace CartSync.Data.Requests;

public record RecipeEntryEditRequest
{
    public required Ulid? PrepId { get; init; }
    public required Amount Amount { get; init; }
}