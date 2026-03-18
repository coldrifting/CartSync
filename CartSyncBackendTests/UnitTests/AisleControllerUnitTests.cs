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
public class AisleControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestGetAisles()
    {
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Contains((SeedData.Aisles[0].AisleId, SeedData.Aisles[0].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((SeedData.Aisles[5].AisleId, SeedData.Aisles[5].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((SeedData.Aisles[13].AisleId, SeedData.Aisles[13].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((SeedData.Aisles[22].AisleId, SeedData.Aisles[22].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.Contains((SeedData.Aisles[23].AisleId, SeedData.Aisles[23].AisleName), aisles2.Select(a => (a.AisleId, a.AisleName)));
    }

    [Fact]
    public async Task TestGetAislesStoreIdNotFound()
    {
        Error error = await AisleController.All(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        await TestGetAisles();
    }
    
    [Fact]
    public async Task TestGetAisleUsage()
    {
        UsageResponse expected = new()
        {
            {
                "Items", 
                [
                    (SeedData.Items[116].ItemId,  SeedData.Items[116].ItemName),
                    (SeedData.Items[117].ItemId,  SeedData.Items[117].ItemName),
                    (SeedData.Items[118].ItemId,  SeedData.Items[118].ItemName),
                    (SeedData.Items[119].ItemId,  SeedData.Items[119].ItemName),
                    (SeedData.Items[120].ItemId,  SeedData.Items[120].ItemName)
                ]
            },
        };
        
        IActionResult result = await AisleController.Usages(SeedData.Stores[0].StoreId, SeedData.Aisles[2].AisleId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }
    
    [Fact]
    public async Task TestGetAisleUsage2()
    {
        UsageResponse expected = new()
        {
            {
                "Items", 
                [
                    (SeedData.Items[0].ItemId,  SeedData.Items[0].ItemName),
                ]
            },
        };
        
        IActionResult result = await AisleController.Usages(SeedData.Stores[1].StoreId, SeedData.Aisles[23].AisleId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }

    [Fact]
    public async Task TestGetAisleUsageBadAisleId()
    {
        Error result = await AisleController.Usages(SeedData.Stores[0].StoreId, Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task TestGetAisleUsageBadStoreId()
    {
        Error result = await AisleController.Usages(Ulid.NotFound, SeedData.Aisles[0].AisleId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task TestGetAisleUsageWrongStoreId()
    {
        Error result = await AisleController.Usages(SeedData.Stores[1].StoreId, SeedData.Aisles[5].AisleId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public async Task TestAddAisle()
    {
        IActionResult result = await AisleController.Add(SeedData.Stores[0].StoreId, new AisleAddRequest
        {
            AisleName = "New Aisle"
        });
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(24, aisles.Count);
        Assert.Contains("New Aisle", aisles.Select(a => a.AisleName));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain("New Aisle", aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestAddAisleInvalidStoreId()
    {
        Error error = await AisleController.Add(Ulid.NotFound, new AisleAddRequest
        {
            AisleName = "New Aisle"
        }).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestEditAisleRename()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/AisleName",
                    value = "New Aisle"
                }
            }
        };
        
        IActionResult result = await AisleController.Edit(SeedData.Stores[0].StoreId, SeedData.Aisles[4].AisleId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Contains("New Aisle", aisles.Select(a => a.AisleName));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain("New Aisle", aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestEditAisleReorderFirst()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/SortOrder",
                    value = 0
                }
            }
        };
        
        IActionResult result = await AisleController.Edit(SeedData.Stores[0].StoreId, SeedData.Aisles[4].AisleId, jsonPatch);
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Equal(SeedData.Aisles[4].AisleName, aisles.OrderBy(a => a.SortOrder).Select(a => a.AisleName).FirstOrDefault());
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain(SeedData.Aisles[4].AisleName, aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestEditAisleInvalidAisleId()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/AisleName",
                    value = "New Aisle"
                }
            }
        };
        
        Error error = await AisleController.Edit(SeedData.Stores[0].StoreId, Ulid.NotFound, jsonPatch).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestDeleteAisle()
    {
        IActionResult result = await AisleController.Delete(SeedData.Stores[0].StoreId, SeedData.Aisles[2].AisleId);
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(22, aisles.Count);
        Assert.DoesNotContain(SeedData.Aisles[2].AisleId, aisles.Select(a => a.AisleId));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain(SeedData.Aisles[2].AisleId, aisles2.Select(a => a.AisleId));
        
        
        IActionResult result2 = await AisleController.Delete(SeedData.Stores[1].StoreId, SeedData.Aisles[23].AisleId);
        Assert.IsType<NoContentResult>(result2);
        
        List<AisleResponse> aisles3 = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Equal(22, aisles3.Count);
        Assert.DoesNotContain(SeedData.Aisles[23].AisleId, aisles3.Select(a => a.AisleId));
        
        List<AisleResponse> aisles4 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync<List<AisleResponse>>();
        Assert.Empty(aisles4);
    }

    [Fact]
    public async Task TestDeleteAisleInvalidAisleId()
    {
        Error error = await AisleController.Delete(SeedData.Stores[0].StoreId, Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestDeleteAisleUnderWrongStore()
    {
        Error error = await AisleController.Delete(SeedData.Stores[1].StoreId, SeedData.Aisles[0].AisleId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestDeleteAisleStoreIdInsteadOfAisleId()
    {
        Error error = await AisleController.Delete(SeedData.Stores[0].StoreId, SeedData.Stores[0].StoreId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);

        await TestGetAisles();
    }
}