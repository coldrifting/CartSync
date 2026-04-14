using CartSync.Data.Entities;

namespace CartSync.SeedData;

// ReSharper disable StringLiteralTypo
public static partial class SeedData
{
    public static List<Aisle> Aisles =>
    [
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPQP582GZ622QTSHVRH"), AisleName = "Aisle 02",        SortOrder = 0 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPRYFRSZEDF468MAZSF"), AisleName = "Aisle 02 Endcap", SortOrder = 1 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSFCBN51KNK5FYBAC5"), AisleName = "Aisle 03",        SortOrder = 2 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSXYCDVP1SZC5VEMFF"), AisleName = "Aisle 04",        SortOrder = 3 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSDQVZ5PH7NY48ECBQ"), AisleName = "Aisle 05",        SortOrder = 4 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPS7CASP7H5E3Z2PW2E"), AisleName = "Aisle 06",        SortOrder = 5 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPS2MCGD4E1SATBQEAE"), AisleName = "Aisle 07",        SortOrder = 6 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSCHXF4NAWM0NCCKP5"), AisleName = "Aisle 08",        SortOrder = 7 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSFPFSG20C25QB77KH"), AisleName = "Aisle 09",        SortOrder = 8 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSPKRMM465G72AQWF5"), AisleName = "Aisle 10",        SortOrder = 9 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPSW2HR3FA34YFMG3SQ"), AisleName = "Aisle 11",        SortOrder = 10 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPS7Y8RGHQQ6ZXGM9H1"), AisleName = "Aisle 12",        SortOrder = 11 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTG3K8ASQHRH7N3PV9"), AisleName = "Aisle 13",        SortOrder = 12 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTG39GK16220VWG9F4"), AisleName = "Aisle 14",        SortOrder = 13 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPT62SQ6A428PNZTEEE"), AisleName = "Aisle 15",        SortOrder = 14 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTZ7N2YM1FCQ0Y9MZS"), AisleName = "Back Right",      SortOrder = 15 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTNW9QF4YXTM9Z0R9Y"), AisleName = "Back Middle",     SortOrder = 16 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTYB6D6CAE76AYX7H0"), AisleName = "Meat",            SortOrder = 17 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTS1DY5NXC6DH220S1"), AisleName = "Frozen Meat",     SortOrder = 18 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTDBSRMCCWQN5KWY8S"), AisleName = "Aisle 01 Endcap", SortOrder = 20 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPT4VBQGS0HN91GC2S5"), AisleName = "Aisle 01",        SortOrder = 21 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPT86FSN79T6BJ54QTX"), AisleName = "Produce",         SortOrder = 22 },
        new() { StoreId = Stores[0].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTQMDDC5ZKS6C2ESB6"), AisleName = "Bakery",          SortOrder = 23 },
        new() { StoreId = Stores[1].StoreId, AisleId = Ulid.Parse("01KJ7DRVPTBKX083VAPHMTPA19"), AisleName = "Example Aisle",   SortOrder = 0 }
    ];
}