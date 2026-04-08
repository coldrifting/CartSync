using System.Net;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.SeedData.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class StoreControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
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
        AddRequest newStore = new()
        {
            Name = "new store"
        };
        
        (StoreResponse store, string location) result = await StoreController.Add(newStore).ValueAsync();
        Assert.Equal(result.store.Name, newStore.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.store.Id.ToString().ToLower());

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Equal(3, stores.Count);
        Assert.Contains("new store", stores.Select(s => s.Name));
    }
    
    [Fact]
    public async Task TestStoreSelection()
    {
        Ulid store0 = SeedData.Stores[0].StoreId;
        Ulid store1 = SeedData.Stores[1].StoreId;
        
        ErrorResponse errorResponse = await StoreController.Delete(SeedData.Stores[0].StoreId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.Conflict);
        
        await StoreController.Select(store1).AssertIsSuccessful();

        List<StoreResponse> storeResponse = await StoreController.All().ValueAsync();
        Assert.False(storeResponse.FirstOrDefault(store => store.Id == store0)?.IsSelected);
        Assert.True(storeResponse.FirstOrDefault(store => store.Id == store1)?.IsSelected);

        await StoreController.Delete(store0).AssertIsSuccessful();
        
        ErrorResponse error2 = await StoreController.Delete(store1).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.Conflict);

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Single(stores);
        Assert.Contains(store1, stores.Select(store => store.Id));
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
                    path = "/Name",
                    value = "edited store"
                }
            }
        };
        
        await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch);

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Equal(2, stores.Count);

        StoreResponse? store = stores.FirstOrDefault(store => store.Id == SeedData.Stores[0].StoreId);
        Assert.NotNull(store);
        Assert.Equal("edited store", store.Name);
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
        
        ErrorResponse errorResponse = await StoreController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("edited store", stores.Select(store => store.Name));
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
        
        ErrorResponse errorResponse = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreName, stores.Select(store => store.Name));
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
        
        ErrorResponse errorResponse = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(store => store.Name));
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
        
        ErrorResponse errorResponse = await StoreController.Edit(SeedData.Stores[0].StoreId, jsonPatch).ErrorAsync();
        
        Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(store => store.Name));
    }
    
    [Fact]
    public async Task TestStoreDelete()
    {
        await StoreController.Delete(SeedData.Stores[1].StoreId).AssertIsSuccessful();

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Single(stores);
        Assert.DoesNotContain(SeedData.Stores[1].StoreId, stores.Select(store => store.Id));
    }
    
    [Fact]
    public async Task TestStoreDelete_SelectedShouldError()
    {
        ErrorResponse errorResponse = await StoreController.Delete(SeedData.Stores[0].StoreId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.Conflict);

        List<StoreResponse> stores = await StoreController.All().ValueAsync();

        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreId, stores.Select(store => store.Id));
    }
    
    [Fact]
    public async Task TestStoreDelete_NotFound()
    {
        ErrorResponse errorResponse = await StoreController.Delete(Ulid.NotFound).ErrorAsync();
        
        Assert.Equal((int)HttpStatusCode.NotFound, errorResponse.StatusCode);
        
        List<StoreResponse> stores = await StoreController.All().ValueAsync();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(SeedData.Stores[0].StoreId, stores.Select(store => store.Id));
        Assert.Contains(SeedData.Stores[1].StoreId, stores.Select(store => store.Id));
    }
}