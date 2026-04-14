using CartSync.Objects;

namespace CartSync.Data.Requests;

public record CartSelectItemEditRequest
{
    public Amount Amount { get; init; } = new();
}