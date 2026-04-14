using CartSync.Data.Entities;

namespace CartSync.SeedData;

public static partial class SeedData
{
    public static List<ItemPrep> ItemPreps =>
    [
        new() { ItemId = Items[178].ItemId, PrepId = Preps[0].PrepId },
        new() { ItemId = Items[216].ItemId, PrepId = Preps[1].PrepId },
        new() { ItemId = Items[215].ItemId, PrepId = Preps[1].PrepId },
        new() { ItemId = Items[207].ItemId, PrepId = Preps[2].PrepId },
        new() { ItemId = Items[179].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[180].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[181].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[182].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[183].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[184].ItemId, PrepId = Preps[3].PrepId },
        new() { ItemId = Items[179].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[180].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[181].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[182].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[183].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[184].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[205].ItemId, PrepId = Preps[4].PrepId },
        new() { ItemId = Items[209].ItemId, PrepId = Preps[5].PrepId },
        new() { ItemId = Items[215].ItemId, PrepId = Preps[5].PrepId },
        new() { ItemId = Items[56].ItemId, PrepId = Preps[6].PrepId },
        new() { ItemId = Items[2].ItemId, PrepId = Preps[7].PrepId }
    ];
}