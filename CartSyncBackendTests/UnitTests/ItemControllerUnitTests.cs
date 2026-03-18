using System.Net;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Database.Seeding;
using CartSyncBackendTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Microsoft.AspNetCore.Mvc;

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

    [Fact]
    public async Task TestGetItemDetailsItemNotFound()
    {
        Error result = await ItemController.Details(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
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

    [Fact]
    public async Task TestGetItemUsage()
    {
        UsageResponse expected = new()
        {
            {
                "Recipes", 
                [
                    (SeedData.Recipes[2].RecipeId,  SeedData.Recipes[2].RecipeName),
                    (SeedData.Recipes[0].RecipeId,  SeedData.Recipes[0].RecipeName)
                ]
            },
        };
        
        IActionResult result = await ItemController.Usages(SeedData.Items[88].ItemId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }

    [Fact]
    public async Task TestGetItemUsageNoUses()
    {
        UsageResponse expected = new();
        
        IActionResult result = await ItemController.Usages(SeedData.Items[22].ItemId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }

    [Fact]
    public async Task TestGetItemUsageBadItemId()
    {
        Error result = await ItemController.Usages(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task TestAddItem()
    {
        ItemAddRequest newItem = new()
        {
            ItemName = "New Item Name",
            DefaultUnitType = UnitType.WeightOunces,
            ItemTemp = ItemTemp.Frozen
        };
        
        ItemResponse result = await ItemController.Add(newItem).CreatedAsync<ItemResponse>(i => i.ItemId);
        Assert.Equal(newItem.ItemName, result.ItemName);
        Assert.Equal(newItem.DefaultUnitType, result.DefaultUnitType);
        Assert.Equal(newItem.ItemTemp, result.ItemTemp);

        List<ItemResponse> results = await ItemController.All(SeedData.Stores[0].StoreId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count + 1, results.Count);
        Assert.Contains(results, r => r.ItemId == result.ItemId);
        
        ItemResponse item = results.First(r => r.ItemId == result.ItemId);
        Assert.Equal(newItem.ItemName, item.ItemName);
        Assert.Equal(newItem.DefaultUnitType, item.DefaultUnitType);
        Assert.Equal(newItem.ItemTemp, item.ItemTemp);
    }

    [Fact]
    public async Task TestEditItemRename()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        List<ItemResponse> items = await ItemController.All(SeedData.Stores[0].StoreId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains("New Item", items.Where(i => i.ItemId == itemId).Select(i => i.ItemName));
    }

    [Fact]
    public async Task TestEditItemTemp()
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

        Ulid storeId = SeedData.Stores[0].StoreId;
        Ulid itemId = SeedData.Items[4].ItemId;
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        List<ItemResponse> items = await ItemController.All(storeId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains(ItemTemp.Frozen, items.Where(i => i.ItemId == itemId).Select(i => i.ItemTemp));
    }

    [Fact]
    public async Task TestEditItemAddLocation()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[1].StoreId);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId, SeedData.Aisles[23].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemUpdateLocation()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[17].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemUpdateLocation2()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[23].AisleId, SeedData.Aisles[17].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddInvalidLocation()
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
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddLocationWithStoreNotFound()
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
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddLocationWithWrongStore()
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
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[4].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemRemoveLocation()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[0].StoreId);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 23)]
    [InlineData(1, 0)]
    public async Task TestEditItemRemoveLocation2(int storeIndex, int aisleIndex)
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch, SeedData.Stores[storeIndex].StoreId);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[aisleIndex].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddPrep()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ prepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddSecondPrep()
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ prepId, SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemAddInvalidPrepId()
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
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(0, 4)]
    [InlineData(1, 3)]
    public async Task TestEditItemRemovePrep(int index, int prepIndex)
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
        
        IActionResult result = await ItemController.Edit(itemId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Preps[prepIndex].PrepId ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemRemoveFromEmptyPrepList()
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
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ ];
        IEnumerable<Ulid> actual = item.Preps.OrderBy(i => i.PrepName).ThenBy(i => i.PrepId).Select(i => i.PrepId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemBadPatch()
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
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync<ItemResponse>();

        IEnumerable<Ulid> expected = [ SeedData.Aisles[0].AisleId, SeedData.Aisles[23].AisleId ];
        IEnumerable<Ulid> actual = item.Locations.OrderBy(i => i.AisleId).Select(i => i.AisleId);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestEditItemInvalidItemId()
    {
        Ulid storeId = SeedData.Stores[0].StoreId;
        
        JsonPatchDocument<AisleEditRequest> jsonPatch = new();
        Error error = await AisleController.Edit(storeId, Ulid.NotFound, jsonPatch).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        List<ItemResponse> items = await ItemController.All(storeId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count, items.Count);
    }

    [Fact]
    public async Task TestEditItemNotFound()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        Error result = await ItemController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task TestEditItemNotFoundWithStoreId()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        Error result = await ItemController.Edit(Ulid.NotFound, jsonPatch, SeedData.Stores[0].StoreId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task TestDeleteItem()
    {
        Ulid storeId = SeedData.Stores[0].StoreId;
        Ulid itemId = SeedData.Items[66].ItemId;
        IActionResult result = await ItemController.Delete(itemId);
        Assert.IsType<NoContentResult>(result);
        
        List<ItemResponse> items = await ItemController.All(storeId).ValueAsync<List<ItemResponse>>();
        Assert.Equal(SeedData.Items.Count - 1, items.Count);
        Assert.DoesNotContain(items, r => r.ItemId == itemId);
    }

    [Fact]
    public async Task TestDeleteItemNotFound()
    {
        Error result = await ItemController.Delete(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
}