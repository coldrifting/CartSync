using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Misc;

public record CartEntryValue
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required AmountGroup Amounts { get; init; }
    public required Ulid? AisleId { get; init; }
    public required Bay Bay { get; init; }
    public required bool IsChecked { get; init; }
    
    public static Expression<Func<CartEntry, CartEntryValue>> FromCartEntry =>
        cartEntry => new CartEntryValue
        {
            ItemId = cartEntry.ItemId,
            PrepId = cartEntry.PrepId,
            Amounts = cartEntry.Amounts,
            AisleId = cartEntry.AisleId,
            Bay = cartEntry.Bay,
            IsChecked = cartEntry.IsChecked
        };
}