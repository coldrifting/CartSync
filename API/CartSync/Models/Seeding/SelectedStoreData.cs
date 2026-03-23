using CartSync.Models.Joins;

namespace CartSync.Models.Seeding;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<SelectedStore> SelectedStores =>
    [
        new()
        {
            UserId = Users[0].UserId, 
            StoreId = Stores[0].StoreId
        }
    ];
}