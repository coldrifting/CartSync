using System.Net;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Database.Seeding;
using CartSyncBackendTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncBackendTests.UnitTests;

[Collection("DatabaseTests")]
public class StoreControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestAllStores()
    {
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
    }
    
    [Fact]
    public async Task TestAddStore()
    {
        StoreAddRequest newStore = new()
        {
            StoreName = "new store"
        };
        
        StoreResponse storeResponse = await StoreController.Add(newStore).CreatedAsync<StoreResponse>(s => s.StoreId);
        Assert.Equal(storeResponse.StoreName, newStore.StoreName);

        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();

        Assert.Equal(3, stores.Count);
        Assert.Contains("new store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestEditStore()
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

        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();

        Assert.Equal(2, stores.Count);

        StoreResponse? store = stores.FirstOrDefault(s => s.StoreId == SeedData.Stores[0].StoreId);
        Assert.NotNull(store);
        Assert.Equal("edited store", store.StoreName);
    }
    
    [Fact]
    public async Task TestEditStoreBadId()
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
        
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("edited store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestEditStoreBadPatch()
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
        
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreName, stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestEditStoreBadNameEmptyString()
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
        
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestEditStoreBadNameNull()
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
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestDeleteStore()
    {
        await StoreController.Delete(SeedData.Stores[0].StoreId);

        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();

        Assert.Single(stores);
        Assert.DoesNotContain(SeedData.Stores[0].StoreId, stores.Select(s => s.StoreId));
    }
    
    [Fact]
    public async Task TestDeleteStoreNotFound()
    {
        Error error = await StoreController.Delete(Ulid.NotFound).ErrorAsync();
        
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreId, stores.Select(s => s.StoreId));
        Assert.Contains(SeedData.Stores[1].StoreId, stores.Select(s => s.StoreId));
    }
}