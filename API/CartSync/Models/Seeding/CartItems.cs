using CartSync.Objects;

namespace CartSync.Models.Seeding;
// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<CartItem> CartItems =>
    [
        new() { ItemId = Items[114].ItemId, PrepId = null, Amount = Amount.Count(3)}
    ];
}