using CartSync.Objects;

namespace CartSync.Data.Responses;

public record CartEntryItemResponse
{
    public required ItemMinimalResponse Item { get; init; }
    public required PrepResponse? Prep { get; init; }
    public required Bay Bay { get; init; }
    public required AmountGroup Amounts { get; init; }
    public required bool IsChecked  { get; init; }
}