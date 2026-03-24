using System.Net;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Models.Joins;
using CartSync.Objects.Enums;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class ItemControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Theory]
    [InlineData(0, 0, 20, 21)]
    [InlineData(1, 23, -1, -1)]
    public async Task TestItemAll(int storeIndex, int aisleIndex1, int aisleIndex2, int aisleIndex3)
    {
        Ulid storeId = SeedData.Stores[storeIndex].StoreId;
        
        await StoreController.Select(storeId);
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[0].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[0].ItemId), 0, [aisleIndex1], 0);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[180].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[180].ItemId), 180, [aisleIndex2], 0);
        
        Assert.Contains(items, ir => ir.ItemId == SeedData.Items[209].ItemId);
        AssertItemEqual(items.Single(i => i.ItemId == SeedData.Items[209].ItemId), 209, [aisleIndex3], 0);
    }

    [Fact]
    public async Task TestItemDetails()
    {
        AssertItemEqual(await ItemController.Details(SeedData.Items[0].ItemId).ValueAsync(), 0, [0, 23]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[30].ItemId).ValueAsync(), 30, [2]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[179].ItemId).ValueAsync(), 179, [20]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[181].ItemId).ValueAsync(), 181,[20]);
        AssertItemEqual(await ItemController.Details(SeedData.Items[209].ItemId).ValueAsync(), 209, [21]);
    }

    [Fact]
    public async Task TestItemDetails_ItemNotFound()
    {
        Error error = await ItemController.Details(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemUsage_OnlyRecipes()
    {
        ItemUsagesResponse expected = new()
        {
            ItemId = SeedData.Items[66].ItemId,
            ItemName = SeedData.Items[66].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[2].RecipeId,
                    RecipeName = SeedData.Recipes[2].RecipeName,
                    Url = SeedData.Recipes[2].Url,
                    IsPinned =  SeedData.Recipes[2].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[0].RecipeId,
                    RecipeName = SeedData.Recipes[0].RecipeName,
                    Url = SeedData.Recipes[0].Url,
                    IsPinned =  SeedData.Recipes[0].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[3].RecipeId,
                    RecipeName = SeedData.Recipes[3].RecipeName,
                    Url = SeedData.Recipes[3].Url,
                    IsPinned =  SeedData.Recipes[3].IsPinned,
                }
            ],
            Preps = []
        };
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[66].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_OnlyPreps()
    {
        ItemUsagesResponse expected = new()
        {
            ItemId = SeedData.Items[2].ItemId,
            ItemName = SeedData.Items[2].ItemName,
            Recipes = [],
            Preps =
            [
                new PrepResponse
                {
                    PrepId = SeedData.Preps[7].PrepId,
                    PrepName = SeedData.Preps[7].PrepName,
                }
            ]
        };
        Assert.Single(expected.Preps);
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[2].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_RecipesAndPreps()
    {
        ItemUsagesResponse expected = new()
        {
            ItemId = SeedData.Items[207].ItemId,
            ItemName = SeedData.Items[207].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[0].RecipeId,
                    RecipeName = SeedData.Recipes[0].RecipeName,
                    Url = SeedData.Recipes[0].Url,
                    IsPinned =  SeedData.Recipes[0].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[3].RecipeId,
                    RecipeName = SeedData.Recipes[3].RecipeName,
                    Url = SeedData.Recipes[3].Url,
                    IsPinned =  SeedData.Recipes[3].IsPinned,
                }
            ],
            Preps = [
                new PrepResponse
                {
                    PrepId = SeedData.Preps[2].PrepId,
                    PrepName = SeedData.Preps[2].PrepName,
                }
            ]
        };
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[207].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_NoUsages()
    {
        ItemUsagesResponse expected = new()
        {
            ItemId = SeedData.Items[22].ItemId,
            ItemName = SeedData.Items[22].ItemName,
            Recipes = [],
            Preps = []
        };
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[22].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_ItemNotFound()
    {
        Error error = await ItemController.Usages(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task TestItemAdd()
    {
        ItemAddRequest newItem = new()
        {
            ItemName = "New Item Name",
        };

        (ItemResponse item, string location) result = await ItemController.Add(newItem).ValueAsync();
        Assert.Equal(newItem.ItemName, result.item.ItemName);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.item.ItemId.ToString().ToLower());

        ItemResponse fetch = await ItemController.Details(result.item.ItemId).ValueAsync();
        Assert.Equal(newItem.ItemName, fetch.ItemName);

        List<ItemByStoreResponse> results = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count + 1, results.Count);
        Assert.Contains(results, r => r.ItemId ==  result.item.ItemId);
        
        ItemByStoreResponse item = results.First(r => r.ItemId == result.item.ItemId);
        Assert.Equal(newItem.ItemName, item.ItemName);
    }

    [Fact]
    public async Task TestItemEdit_Rename()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/ItemName",
                    value = "New Item"
                }
            }
        };

        Ulid itemId = SeedData.Items[5].ItemId;
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains("New Item", items.Where(i => i.ItemId == itemId).Select(i => i.ItemName));
    }

    [Fact]
    public async Task TestItemEdit_Temp()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/ItemTemp",
                    value = "Frozen"
                }
            }
        };

        Ulid itemId = SeedData.Items[4].ItemId;
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains(ItemTemp.Frozen, items.Where(i => i.ItemId == itemId).Select(i => i.ItemTemp));
    }

    [Fact]
    public async Task TestItemEdit_AddLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid aisleId = SeedData.Aisles[23].AisleId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[1].StoreId).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId, SeedData.Aisles[23].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_UpdateLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid aisleId = SeedData.Aisles[17].AisleId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[17].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_UpdateLocation2()
    {
        Ulid itemId = SeedData.Items[0].ItemId;
        Ulid aisleId = SeedData.Aisles[17].AisleId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[23].AisleId, SeedData.Aisles[17].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddInvalidLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid aisleId = Ulid.NotFound;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddLocationWithStoreNotFound()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid aisleId = SeedData.Aisles[4].AisleId;
        Ulid storeId = Ulid.NotFound;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch, storeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddLocationWithWrongStore()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid aisleId = SeedData.Aisles[4].AisleId;
        Ulid storeId = SeedData.Stores[1].StoreId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = $"{aisleId}"
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch, storeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_RemoveLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = null
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 23)]
    [InlineData(1, 0)]
    public async Task TestItemEdit_RemoveLocation2(int storeIndex, int aisleIndex)
    {
        Ulid itemId = SeedData.Items[0].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/AisleId",
                    value = null
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[storeIndex].StoreId).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[aisleIndex].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddPrep()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid prepId = SeedData.Preps[3].PrepId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "add",
                    path = "/PrepIds/-",
                    value = $"{prepId}"
                }
            }
        };

        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddSecondPrep()
    {
        Ulid itemId = SeedData.Items[180].ItemId;
        Ulid prepId = SeedData.Preps[1].PrepId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "add",
                    path = "/PrepIds/-",
                    value = $"{prepId}"
                }
            }
        };

        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId, SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_AddInvalidPrepShouldError()
    {
        Ulid itemId = SeedData.Items[180].ItemId;
        Ulid prepId = Ulid.NotFound;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "add",
                    path = "/PrepIds/-",
                    value = $"{prepId}"
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(0, 4)]
    [InlineData(1, 3)]
    public async Task TestItemEdit_RemovePrep(int index, int prepIndex)
    {
        Ulid itemId = SeedData.Items[180].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "remove",
                    path = $"/PrepIds/{index}"
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Preps[prepIndex].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_RemoveFromEmptyPrepListShouldError()
    {
        Ulid itemId = SeedData.Items[30].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "remove",
                    path = $"/PrepIds/0"
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_BadPatch()
    {
        Ulid itemId = SeedData.Items[0].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/ItemName",
                    value = null
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[0].AisleId, SeedData.Aisles[23].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_ItemNotFound()
    {
        Ulid storeId = SeedData.Stores[0].StoreId;
        
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        Error error = await ItemController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        Error error2 = await ItemController.Edit(Ulid.NotFound, jsonPatch, storeId).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemEdit_StoreNotFound()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        Error error = await ItemController.Edit(Ulid.NotFound, jsonPatch, SeedData.Stores[0].StoreId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemDelete()
    {
        Ulid itemId = SeedData.Items[66].ItemId;
        await ItemController.Delete(itemId).AssertIsSuccessful();
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count - 1, items.Count);
        Assert.DoesNotContain(items, r => r.ItemId == itemId);
    }

    [Fact]
    public async Task TestItemDelete_ItemNotFound()
    {
        Error error = await ItemController.Delete(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    // Helper function
    private static void AssertItemEqual(ItemByStoreResponse itemResponse, int itemIndex, int[]? aisleIndices = null, int? storeIndex = null)
    {
        ItemResponse item = new()
        {
            ItemId = itemResponse.ItemId,
            ItemName = itemResponse.ItemName,
            ItemTemp =  itemResponse.ItemTemp,
            DefaultUnitType = itemResponse.DefaultUnitType,
            Preps = itemResponse.Preps,
            Locations = itemResponse.Location != null ? [itemResponse.Location] : [],
        };
        AssertItemEqual(item, itemIndex, aisleIndices, storeIndex);
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
            
            // TODO - Test item locations in a better way
            /*
            List<ItemAisleResponse> expectedAisles = aisleIds
                .Select(aisleId => SeedData.Aisles.Single(aisle => aisle.AisleId == aisleId))
                .AsQueryable()
                .Select(Aisle.ToResponse)
                .Select(a => new ItemAisleResponse
                {
                    AisleId = a.AisleId,
                    AisleName = a.AisleName,
                    StoreId = a.StoreId,
                    SortOrder = a.SortOrder,
                    Bay = BayType.Middle
                })
                .OrderBy(a => a.AisleName)
                .ThenBy(a => a.AisleId)
                .ToList();

            Assert.Equal(expectedAisles, itemResponse.Locations);
            */
        }
    }
}