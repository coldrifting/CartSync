using CartSync.Objects;
using CartSync.Objects.Enums;

namespace CartSync.Models.Seeding;
// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<CartItem> CartItems =>
    [
        new() { ItemId = Items[114].ItemId, PrepId = null, Amount = new Amount(3, UnitType.Count)}
    ];
}