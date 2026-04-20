using CartSync.Data.Entities;

namespace CartSync.SeedData;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<UserInfo> SelectedStores =>
    [
        new()
        {
            UserId = Users[0].UserId, 
            StoreId = Stores[0].StoreId
        }
    ];
}