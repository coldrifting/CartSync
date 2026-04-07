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
        
        Assert.Contains(items, item => item.Id == SeedData.Items[0].ItemId);
        AssertItemEqual(items.Single(item => item.Id == SeedData.Items[0].ItemId), 0, [aisleIndex1]);
        
        Assert.Contains(items, item => item.Id == SeedData.Items[180].ItemId);
        AssertItemEqual(items.Single(item => item.Id == SeedData.Items[180].ItemId), 180, [aisleIndex2]);
        
        Assert.Contains(items, item => item.Id == SeedData.Items[209].ItemId);
        AssertItemEqual(items.Single(item => item.Id == SeedData.Items[209].ItemId), 209, [aisleIndex3]);
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
            Id = SeedData.Items[66].ItemId,
            Name = SeedData.Items[66].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[2].RecipeId,
                    Name = SeedData.Recipes[2].RecipeName,
                    IsPinned =  SeedData.Recipes[2].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[0].RecipeId,
                    Name = SeedData.Recipes[0].RecipeName,
                    IsPinned =  SeedData.Recipes[0].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[3].RecipeId,
                    Name = SeedData.Recipes[3].RecipeName,
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
            Id = SeedData.Items[2].ItemId,
            Name = SeedData.Items[2].ItemName,
            Recipes = [],
            Preps =
            [
                new PrepResponse
                {
                    Id = SeedData.Preps[7].PrepId,
                    Name = SeedData.Preps[7].PrepName,
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
            Id = SeedData.Items[207].ItemId,
            Name = SeedData.Items[207].ItemName,
            Recipes = [
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[0].RecipeId,
                    Name = SeedData.Recipes[0].RecipeName,
                    IsPinned =  SeedData.Recipes[0].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[3].RecipeId,
                    Name = SeedData.Recipes[3].RecipeName,
                    IsPinned =  SeedData.Recipes[3].IsPinned,
                }
            ],
            Preps = [
                new PrepResponse
                {
                    Id = SeedData.Preps[2].PrepId,
                    Name = SeedData.Preps[2].PrepName,
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
            Name = "New Item Name",
        };

        (ItemResponse item, string location) result = await ItemController.Add(newItem).ValueAsync();
        Assert.Equal(newItem.Name, result.item.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.item.Id.ToString().ToLower());

        ItemByStoreResponse fetch = await ItemController.Details(result.item.Id).ValueAsync();
        Assert.Equal(newItem.Name, fetch.Name);

        List<ItemByStoreResponse> results = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count + 1, results.Count);
        Assert.Contains(results, item => item.Id == result.item.Id);
        
        ItemByStoreResponse item = results.First(item => item.Id == result.item.Id);
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
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
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
                    value = "Frozen"
                }
            }
        };

        Ulid itemId = SeedData.Items[4].ItemId;
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count, items.Count);
        Assert.Contains(Temp.Frozen, items.Where(item => item.Id == itemId).Select(i => i.Temp));
    }

    [Fact]
    public async Task TestItemEdit_AddLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        
        ItemAisleEditRequest location = new()
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
                    value = location
                }
            }
        };

        await StoreController.Select(SeedData.Stores[1].StoreId);
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Assert.Equal(location.AisleId, item.Location?.AisleId);
        Assert.Equal(location.Bay, item.Location?.Bay);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(54)]
    public async Task TestItemEdit_UpdateLocation(int itemIndex)
    {
        Ulid itemId = SeedData.Items[itemIndex].ItemId;
        
        ItemAisleEditRequest location = new()
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
                    value = location
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Assert.Equal(location.AisleId, item.Location?.AisleId);
        Assert.Equal(location.Bay, item.Location?.Bay);
    }

    [Fact]
    public async Task TestItemEdit_AddInvalidLocation()
    {
        Ulid itemId = SeedData.Items[54].ItemId;
        
        ItemAisleEditRequest location = new()
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
                    value = location
                }
            }
        };
        
        Error error = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Assert.Equal(SeedData.Aisles[4].AisleId, item.Location?.AisleId);
        Assert.Equal(Bay.Center, item.Location?.Bay);
    }

    [Fact]
    public async Task TestItemEdit_AddLocationWithWrongStore()
    {
        Ulid itemId = SeedData.Items[54].ItemId;

        ItemAisleEditRequest location = new()
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
                    value = location
                }
            }
        };
        
        await StoreController.Select(SeedData.Stores[1].StoreId);
        
        Error error = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Assert.Null(item.Location?.AisleId);
        Assert.Null(item.Location?.Bay);
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
                    path = "/Location",
                    value = null
                }
            }
        };
        
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Assert.Null(item.Location?.AisleId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task TestItemEdit_RemoveLocation2(int storeIndex)
    {
        Ulid itemId = SeedData.Items[0].ItemId;
        JsonPatchDocument<ItemEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<ItemEditRequest>
                {
                    op = "replace",
                    path = "/Location",
                    value = null
                }
            }
        };
        
        await StoreController.Select(SeedData.Stores[storeIndex].StoreId);
        await ItemController.Edit(itemId, jsonPatch).AssertIsSuccessful();
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Ulid? expected = null;
        Ulid? actual = item.Location?.AisleId;
        
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
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
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
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ prepId, SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
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
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Preps[3].PrepId, SeedData.Preps[4].PrepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
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
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ SeedData.Preps[prepIndex].PrepId ];
        IEnumerable<Ulid> actual = item.Preps
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .Select(prep => prep.Id);
        
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
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        IEnumerable<Ulid> expected = [ ];
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
        
        Error error = await ItemController.Edit(itemId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        ItemByStoreResponse item = await ItemController.Details(itemId).ValueAsync();

        Ulid expected = SeedData.Aisles[0].AisleId;
        Ulid? actual = item.Location?.AisleId;
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task TestItemEdit_ItemNotFound()
    {
        JsonPatchDocument<ItemEditRequest> jsonPatch = new();
        Error error = await ItemController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestItemDelete()
    {
        Ulid itemId = SeedData.Items[66].ItemId;
        await ItemController.Delete(itemId).AssertIsSuccessful();
        
        List<ItemByStoreResponse> items = await ItemController.All().ValueAsync();
        Assert.Equal(SeedData.Items.Count - 1, items.Count);
        Assert.DoesNotContain(items, item => item.Id == itemId);
    }

    [Fact]
    public async Task TestItemDelete_ItemNotFound()
    {
        Error error = await ItemController.Delete(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    // Helper function
    private static void AssertItemEqual(ItemByStoreResponse itemResponse, int itemIndex, int[]? aisleIndices = null)
    {
        ItemResponse item = new()
        {
            Id = itemResponse.Id,
            Name = itemResponse.Name,
            Temp =  itemResponse.Temp,
            DefaultUnitType = itemResponse.DefaultUnitType,
            UncapCartUnits = itemResponse.UncapCartUnits,
            Preps = itemResponse.Preps,
            Locations = itemResponse.Location != null ? [itemResponse.Location] : [],
        };
        AssertItemEqual(item, itemIndex, aisleIndices);
    }

    private static void AssertItemEqual(ItemResponse itemResponse, int itemIndex, int[]? aisleIndices = null)
    {
        Assert.Equal(SeedData.Items[itemIndex].ItemId, itemResponse.Id);
        Assert.Equal(SeedData.Items[itemIndex].ItemName, itemResponse.Name);
        Assert.Equal(SeedData.Items[itemIndex].Temp, itemResponse.Temp);
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
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToList();
        
        Assert.Equal(itemResponse.Preps, expectedPreps);

        if (aisleIndices is null || aisleIndices.Length == 0 || aisleIndices is [-1])
        {
            Assert.Empty(itemResponse.Locations);
        }
    }
}