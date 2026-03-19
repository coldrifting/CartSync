namespace CartSyncBackend.Models.Seeding;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<Store> Stores =>
    [
        new() { StoreId = Ulid.Parse("01KJ7DEC2YQFR6XDT0EPGK7Y76"), StoreName = "Macey's (1700 S)" },
        new() { StoreId = Ulid.Parse("01KJ7DGWP88JC7FREBKZAS0M96"), StoreName = "WinCo (2100 S)" }
    ];
}