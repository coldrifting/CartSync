using System.Net;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Objects;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.SeedData.SeedData;

namespace CartSyncTests.ControllerTests;

public readonly struct ItemIndexLocation(int itemIndex, IndexedLocation? location)
{
    public readonly int ItemIndex = itemIndex;
    public readonly IndexedLocation? Location = location;
}

public readonly struct IndexedLocation(int aisleIndex, Bay bay)
{
    public readonly int AisleIndex = aisleIndex;
    public readonly Bay Bay = bay;
}

[Collection("DatabaseTests")]
public class ItemControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task TestItemAll(int storeIndex)
    {
        ItemIndexLocation[] expectedLocations =
            storeIndex == 0
                ?
                [
                    new ItemIndexLocation(0, new IndexedLocation(0, Bay.Center)),
                    new ItemIndexLocation(30, new IndexedLocation(2, Bay.Begin)),
                    new ItemIndexLocation(41, new IndexedLocation(2, Bay.End)),
                    new ItemIndexLocation(180, new IndexedLocation(20, Bay.Center)),
                    new ItemIndexLocation(209, new IndexedLocation(21, Bay.Center)),
                ]
                :
                [
                    new ItemIndexLocation(0, new IndexedLocation(23, Bay.Begin)),
                    new ItemIndexLocation(30, null),
                    new ItemIndexLocation(41, null),
                    new ItemIndexLocation(180, null),
                    new ItemIndexLocation(209, null),
                ];

        Ulid storeId = SeedData.Stores[storeIndex].StoreId;
        await StoreController.Select(storeId);
        ReadOnlyList<ItemResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);

        foreach (ItemIndexLocation locationData in expectedLocations)
        {
            ItemResponse? item = items.FirstOrDefault(item => item.Id == SeedData.Items[locationData.ItemIndex].ItemId);
            Assert.NotNull(item);
            AssertItemEqual(item, locationData.ItemIndex, locationData.Location);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task TestItemDetails(int storeIndex)
    {
        ItemIndexLocation[] expectedLocations =
            storeIndex == 0
                ?
                [
                    new ItemIndexLocation(0, new IndexedLocation(0, Bay.Center)),
                    new ItemIndexLocation(30, new IndexedLocation(2, Bay.Begin)),
                    new ItemIndexLocation(41, new IndexedLocation(2, Bay.End)),
                    new ItemIndexLocation(179, new IndexedLocation(20, Bay.Center)),
                    new ItemIndexLocation(181, new IndexedLocation(20, Bay.Center)),
                    new ItemIndexLocation(209, new IndexedLocation(21, Bay.Center)),
                ]
                :
                [
                    new ItemIndexLocation(0, new IndexedLocation(23, Bay.Begin)),
                    new ItemIndexLocation(30, null),
                    new ItemIndexLocation(179, null),
                    new ItemIndexLocation(181, null),
                    new ItemIndexLocation(209, null),
                ];
        
        Ulid storeId = SeedData.Stores[storeIndex].StoreId;
        await StoreController.Select(storeId);
        
        foreach (ItemIndexLocation locationData in expectedLocations)
        {
            ItemResponse item = await ItemController.Details(SeedData.Items[locationData.ItemIndex].ItemId).ValueAsync();
            AssertItemEqual(item, locationData.ItemIndex, locationData.Location);
        }
    }

    [Fact]
    public async Task TestItemDetails_ItemNotFound()
    {
        ErrorResponse errorResponse = await ItemController.Details(Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemUsage_OnlyRecipes()
    {
        ItemUsagesResponse expected = new()
        {
            Id = SeedData.Items[66].ItemId,
            Name = SeedData.Items[66].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[2].RecipeId,
                    Name = SeedData.Recipes[2].RecipeName,
                    IsPinned = SeedData.Recipes[2].IsPinned,
                    Url = SeedData.Recipes[2].Url
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[0].RecipeId,
                    Name = SeedData.Recipes[0].RecipeName,
                    IsPinned = SeedData.Recipes[0].IsPinned,
                    Url = SeedData.Recipes[0].Url
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[3].RecipeId,
                    Name = SeedData.Recipes[3].RecipeName,
                    IsPinned = SeedData.Recipes[3].IsPinned,
                    Url = SeedData.Recipes[3].Url
                }
            ]
        };
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[66].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_RecipesAndPreps()
    {
        ItemUsagesResponse expected = new()
        {
            Id = SeedData.Items[207].ItemId,
            Name = SeedData.Items[207].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[0].RecipeId,
                    Name = SeedData.Recipes[0].RecipeName,
                    IsPinned = SeedData.Recipes[0].IsPinned,
                    Url = SeedData.Recipes[0].Url
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[3].RecipeId,
                    Name = SeedData.Recipes[3].RecipeName,
                    IsPinned = SeedData.Recipes[3].IsPinned,
                    Url = SeedData.Recipes[3].Url
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
            Id = SeedData.Items[22].ItemId,
            Name = SeedData.Items[22].ItemName,
            Recipes = []
        };
        
        ItemUsagesResponse result = await ItemController.Usages(SeedData.Items[22].ItemId).ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestItemUsage_ItemNotFound()
    {
        ErrorResponse errorResponse = await ItemController.Usages(Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task TestItemAdd()
    {
        AddRequest newItem = new()
        {
            Name = "New Item Name",
        };

        (ItemMinimalResponse item, string location) result = await ItemController.Add(newItem).ValueAsync();
        Assert.Equal(newItem.Name, result.item.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.item.Id.ToString().ToLower());

        ItemResponse fetch = await ItemController.Details(result.item.Id).ValueAsync();
        Assert.Equal(newItem.Name, fetch.Name);

        ReadOnlyList<ItemResponse> results = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count + 1, results.Count);
        Assert.Contains(results, item => item.Id == result.item.Id);
        
        ItemResponse item = results.First(item => item.Id == result.item.Id);
        Assert.Equal(newItem.Name, item.Name);
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
                    path = "/Name",
                    value = "New Item"
                }
            }
        };

        Ulid itemId = SeedData.Items[5].ItemId;
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ReadOnlyList<ItemResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains("New Item", items.Where(item => item.Id == itemId).Select(i => i.Name));
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
                    path = "/Temp",
                    value = Temp.Frozen
                }
            }
        };

        Ulid itemId = SeedData.Items[4].ItemId;
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ReadOnlyList<ItemResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains(Temp.Frozen, items.Where(item => item.Id == itemId).Select(i => i.Temp));
    }

    [Fact]
    public async Task TestItemEdit_AddLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        
        LocationEditRequest locationEditRequest = new()
        {
            AisleId = SeedData.Aisles[23].AisleId,
            Bay = Bay.End
        };
        
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/Location",
                    value = locationEditRequest
                }
            }
        };

        await StoreController.Select(SeedData.Stores[1].StoreId);
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[1].StoreId);
        Assert.NotNull(location);
        
        Assert.Equal(locationEditRequest.AisleId, location.AisleId);
        Assert.Equal(locationEditRequest.Bay, location.Bay);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(54)]
    public async Task TestItemEdit_UpdateLocation(int itemIndex)
    {
        Ulid itemId = SeedData.Items[itemIndex].ItemId;
        
        LocationEditRequest locationEditRequest = new()
        {
            AisleId = SeedData.Aisles[17].AisleId,
            Bay = Bay.Begin
        };
        
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/Location",
                    value = locationEditRequest
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();
        
        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[0].StoreId);
        Assert.NotNull(location);
        
        Assert.Equal(locationEditRequest.AisleId, location.AisleId);
        Assert.Equal(locationEditRequest.Bay, location.Bay);
    }

    [Fact]
    public async Task TestItemEdit_AddInvalidLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        
        LocationEditRequest locationEditRequest = new()
        {
            AisleId = Ulid.NotFound,
            Bay = Bay.End
        };
        
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/Location",
                    value = locationEditRequest
                }
            }
        };
        
        ErrorResponse errorResponse = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[0].StoreId);
        Assert.NotNull(location);
        
        Assert.Equal(SeedData.Aisles[4].AisleId, location.AisleId);
        Assert.Equal(Bay.Center, location.Bay);
    }

    [Fact]
    public async Task TestItemEdit_AddLocationWithWrongStore()
    {
        Ulid itemId = SeedData.Items[54].ItemId;

        LocationEditRequest locationEditRequest = new()
        {
            AisleId = SeedData.Aisles[4].AisleId,
            Bay = Bay.End
        };
        
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/Location",
                    value = locationEditRequest
                }
            }
        };
        
        await StoreController.Select(SeedData.Stores[1].StoreId);
        
        ErrorResponse errorResponse = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[1].StoreId);
        Assert.Null(location);
    }

    [Fact]
    public async Task TestItemEditPrep_RemoveLocations()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        ItemPrepEditRequest editRequest = new()
        {
            PrepIds = []
        };
        
        await ItemController.EditPreps(itemId, editRequest).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[1].StoreId);
        Assert.Null(location);
    }

    [Fact]
    public async Task TestItemEditPrep_AddPrep()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        Ulid prepId = SeedData.Preps[3].PrepId;
        ItemPrepEditRequest editRequest = new()
        {
            PrepIds = [prepId]
        };

        await ItemController.EditPreps(itemId, editRequest).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEditPrep_AddTwoPreps()
    {
        Ulid itemId = SeedData.Items[180].ItemId;
        Ulid prepId = SeedData.Preps[1].PrepId;
        Ulid prepId2 = SeedData.Preps[5].PrepId;
        ItemPrepEditRequest editRequest = new()
        {
            PrepIds = [prepId, prepId2]
        };

        await ItemController.EditPreps(itemId, editRequest).AssertIsSuccessful();
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId, prepId2 ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEditPrep_AddInvalidPrepShouldError()
    {
        Ulid itemId = SeedData.Items[180].ItemId;
        ItemPrepEditRequest editRequest = new ItemPrepEditRequest()
        {
            PrepIds = [Ulid.NotFound]
        };
        
        ErrorResponse errorResponse = await ItemController.EditPreps(itemId, editRequest).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
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
        
        ErrorResponse errorResponse = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        ItemResponse item = await ItemController.Details(itemId).ValueAsync();
        
        ItemAisleResponse? location = item.Locations.FirstOrDefault(itemAisle => itemAisle.StoreId == SeedData.Stores[0].StoreId);
        Assert.NotNull(location);
        
        Assert.Equal(SeedData.Aisles[0].AisleId, location.AisleId);
    }

    [Fact]
    public async Task TestItemEdit_ItemNotFound()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        ErrorResponse errorResponse = await ItemController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemDelete()
    {
        Ulid itemId = SeedData.Items[66].ItemId;
        await ItemController.Delete(itemId).AssertIsSuccessful();
        
        ReadOnlyList<ItemResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count - 1, items.Count);
        Assert.DoesNotContain(items, item => item.Id == itemId);
    }

    [Fact]
    public async Task TestItemDelete_ItemNotFound()
    {
        ErrorResponse errorResponse = await ItemController.Delete(Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }

    // Helper function
    private static void AssertItemEqual(ItemResponse itemResponse, int itemIndex, IndexedLocation? location)
    {
        Assert.Equal(SeedData.Items[itemIndex].ItemId, itemResponse.Id);
        Assert.Equal(SeedData.Items[itemIndex].ItemName, itemResponse.Name);
        Assert.Equal(SeedData.Items[itemIndex].Temp, itemResponse.Temp);
        Assert.Equal(SeedData.Items[itemIndex].DefaultUnitType, itemResponse.DefaultUnitType);

        if (itemResponse.Preps.Count > 0)
        {
            ReadOnlyList<PrepResponse> expectedPreps = SeedData.ItemPreps
                .Where(ip => ip.ItemId == SeedData.Items[itemIndex].ItemId)
                .Select(ip => SeedData.Preps.Single(p => p.PrepId == ip.PrepId))
                .AsQueryable()
                .Select(PrepResponse.FromEntity)
                .OrderBy(prep => prep.Name)
                .ThenBy(prep => prep.Id)
                .ToReadOnlyList();
            
            Assert.Equal(itemResponse.Preps, expectedPreps);
        }

        if (location == null)
        {
            return;
        }

        ItemAisleResponse expected = new()
        {
            AisleId = SeedData.Aisles[location.Value.AisleIndex].AisleId,
            AisleName = SeedData.Aisles[location.Value.AisleIndex].AisleName,
            Bay = location.Value.Bay,
            StoreId = Ulid.Empty,
            SortOrder = 0
        };

        ReadOnlyList<ItemAisleResponse> actual = itemResponse.Locations.Select(itemAisle => itemAisle with { StoreId = Ulid.Empty, SortOrder = 0 }).ToReadOnlyList();
            
        Assert.Contains(actual, itemAisle => itemAisle == expected);
    }
}