using CartSync.Objects;

namespace CartSync.Models.Seeding;
// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<CartSelectItem> CartItems =>
    [
        new() { ItemId = Items[114].ItemId, PrepId = null, Amounts = Amount.Count(3)},
        new() { ItemId = Items[181].ItemId, PrepId = Preps[4].PrepId, Amounts = Amount.VolumeCups(2)},
        new() { ItemId = Items[183].ItemId, PrepId = null, Amounts = Amount.VolumeCups(8)},
        new() { ItemId = Items[183].ItemId, PrepId = Preps[3].PrepId, Amounts = Amount.VolumeCups(4)}
    ];
}