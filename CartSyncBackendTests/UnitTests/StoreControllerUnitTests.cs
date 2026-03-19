using System.Net;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models;
using CartSyncBackendTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSyncBackend.Models.Seeding.SeedData;

namespace CartSyncBackendTests.UnitTests;

[Collection("DatabaseTests")]
public class StoreControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestStoreAll()
    {
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
    }
    
    [Fact]
    public async Task TestStoreAdd()
    {
        StoreAddRequest newStore = new()
        {
            StoreName = "new store"
        };
        
        (StoreResponse store, string location) result = await StoreController.Add(newStore).ValueAsync();
        Assert.Equal(result.store.StoreName, newStore.StoreName);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.store.StoreId.ToString().ToLower());

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Equal(3, stores.Count);
        Assert.Contains("new store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestStoreEdit_Rename()
    {
        JsonPatchDocument<StoreEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<StoreEditRequest>
                {
                    op = "replace",
                    path = "/StoreName",
                    value = "edited store"
                }
            }
        };
        
        await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch);

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Equal(2, stores.Count);

        StoreResponse? store = stores.FirstOrDefault(s => s.StoreId == SeedData.Stores[0].StoreId);
        Assert.NotNull(store);
        Assert.Equal("edited store", store.StoreName);
    }
    
    [Fact]
    public async Task TestStoreEdit_StoreNotFound()
    {
        JsonPatchDocument<StoreEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<StoreEditRequest>
                {
                    op = "replace",
                    path = "/StoreName",
                    value = "edited store"
                }
            }
        };
        
        Error error = await StoreController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("edited store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestStoreEdit_RemoveStoreNameShouldError()
    {
        JsonPatchDocument<StoreEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<StoreEditRequest>
                {
                    op = "remove",
                    path = "/StoreName"
                }
            }
        };
        
        Error error = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreName, stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestStoreEdit_StoreNameEmptyShouldError()
    {
        JsonPatchDocument<StoreEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<StoreEditRequest>
                {
                    op = "replace",
                    path = "/StoreName",
                    value = ""
                }
            }
        };
        
        Error error = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestStoreEdit_StoreNameNullShouldError()
    {
        JsonPatchDocument<StoreEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<StoreEditRequest>
                {
                    op = "replace",
                    path = "/StoreName",
                    value = null
                }
            }
        };
        
        Error error = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestStoreDelete()
    {
        await StoreController.Delete(SeedData.Stores[0].StoreId);

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Single(stores);
        Assert.DoesNotContain(SeedData.Stores[0].StoreId, stores.Select(s => s.StoreId));
    }
    
    [Fact]
    public async Task TestStoreDelete_NotFound()
    {
        Error error = await StoreController.Delete(Ulid.NotFound).ErrorAsync();
        
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreId, stores.Select(s => s.StoreId));
        Assert.Contains(SeedData.Stores[1].StoreId, stores.Select(s => s.StoreId));
    }
}