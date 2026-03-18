using System.Net;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Database.Seeding;
using CartSyncBackendTests.Core;

namespace CartSyncBackendTests.UnitTests;

[Collection("DatabaseTests")]
public class ItemControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Theory]
    [InlineData(0, 0, 20, 21)]
    [InlineData(1, 23, -1, -1)]
    public async Task TestGetAllItems(int storeIndex, int aisleIndex1, int aisleIndex2, int aisleIndex3)
    {
        List<ItemResponse> items = await ItemController.All(SeedData.Stores[storeIndex].StoreId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count, items.Count);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[0].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[0].ItemId), 0, [aisleIndex1], 0);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[180].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[180].ItemId), 180, [aisleIndex2], 0);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[209].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[209].ItemId), 209, [aisleIndex3], 0);
    }

    [Fact]
    public async Task TestGetAllItemsInvalidStoreId()
    {
        Error error = await ItemController.All(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
    }

    [Fact]
    public async Task TestGetItemDetails()
    {
        AssertItemEqual(await ItemController.Details(SeedData.Items[0].ItemId).ValueAsync<ItemResponse>(), 0, [0, 23]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[30].ItemId).ValueAsync<ItemResponse>(), 30, [2]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[179].ItemId).ValueAsync<ItemResponse>(), 179, [20]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[181].ItemId).ValueAsync<ItemResponse>(), 181,[20]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[209].ItemId).ValueAsync<ItemResponse>(), 209, [21]);
    }

    private static void AssertItemEqual(ItemResponse itemResponse, int itemIndex, int[]? aisleIndices = null, int? storeIndex = null)
    {
        Assert.Equal(SeedData.Items[itemIndex].ItemId, itemResponse.ItemId);
        Assert.Equal(SeedData.Items[itemIndex].ItemName, itemResponse.ItemName);
        Assert.Equal(SeedData.Items[itemIndex].ItemTemp, itemResponse.ItemTemp);
        Assert.Equal(SeedData.Items[itemIndex].DefaultUnitType, itemResponse.DefaultUnitType);

        if (itemResponse.Preps.Count <= 0)
        {
            return;
        }

        List<PrepResponse> expectedPreps = SeedData.ItemPreps
            .Where(ip => ip.ItemId == SeedData.Items[itemIndex].ItemId)
            .Select(ip => SeedData.Preps.Single(p => p.PrepId == ip.PrepId))
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        Assert.Equal(itemResponse.Preps, expectedPreps);

        if (aisleIndices is null || aisleIndices.Length == 0 || aisleIndices is [-1])
        {
            Assert.Empty(itemResponse.Locations);
        }
        else
        {
            HashSet<Ulid> aisleIds = [];
            foreach (ItemAisle itemAisle in SeedData.ItemAisles)
            {
                foreach (int aisleIndex in aisleIndices)
                {
                    if (itemAisle.AisleId != SeedData.Aisles[aisleIndex].AisleId)
                    {
                        continue;
                    }

                    if (storeIndex == null || itemAisle.StoreId == SeedData.Stores[storeIndex.Value].StoreId)
                    {
                        aisleIds.Add(itemAisle.AisleId);
                    }
                }
            }
            
            List<AisleResponse> expectedAisles = aisleIds
                .Select(aisleId => SeedData.Aisles.Single(aisle => aisle.AisleId == aisleId))
                .AsQueryable()
                .Select(Aisle.ToResponse)
                .OrderBy(a => a.AisleName)
                .ThenBy(a => a.AisleId)
                .ToList();

            Assert.Equal(expectedAisles, itemResponse.Locations);
        }
    }
}