using CartSyncBackend.Controllers;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackendTests.Core;
using static CartSyncBackend.Database.ExampleData;
using static CartSyncBackendTests.Core.Constants;

namespace CartSyncBackendTests;

[Collection("DatabaseUnitTests")]
public class StoreControllerTests(DatabaseFixture fixture) : DatabaseControllerFixture(fixture)
{
    private StoreController _storeController = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _storeController = new StoreController(Context);
    }

    [Fact]
    public async Task TestAllStores()
    {
        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
    }
    
    [Fact]
    public async Task TestAddStore()
    {
        await _storeController.Add("new store");

        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();

        Assert.Equal(3, stores.Count);
        Assert.Contains("new store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestAddStoreBadNameEmptyString()
    {
        Error error = (await _storeController.Add("")).Error();

        Assert.Equal(BadRequestStatusCode, error.Status);
    }
    
    [Fact]
    public async Task TestEditStore()
    {
        await _storeController.Edit(Stores[0].StoreId, "edited store");

        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();

        Assert.Equal(2, stores.Count);

        StoreResponse? store = stores.FirstOrDefault(s => s.StoreId == Stores[0].StoreId);
        Assert.NotNull(store);
        Assert.Equal("edited store", store.StoreName);
    }
    
    [Fact]
    public async Task TestEditStoreBadId()
    {
        Error error = (await _storeController.Edit(BadId, "edited store")).Error();
        
        Assert.Equal(NotFoundStatusCode, error.Status);
        
        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("edited store", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestEditStoreBadNameEmptyString()
    {
        Error error = (await _storeController.Edit(Stores[0].StoreId, "")).Error();
        
        Assert.Equal(BadRequestStatusCode, error.Status);
        
        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.DoesNotContain("", stores.Select(s => s.StoreName));
    }
    
    [Fact]
    public async Task TestDeleteStore()
    {
        await _storeController.Delete(Stores[0].StoreId);

        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();

        Assert.Single(stores);
        Assert.DoesNotContain(Stores[0].StoreId, stores.Select(s => s.StoreId));
    }
    
    [Fact]
    public async Task TestDeleteStoreNotFound()
    {
        Error error = (await _storeController.Delete(BadId)).Error();
        
        Assert.Equal(NotFoundStatusCode, error.Status);
        
        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(Stores[0].StoreId, stores.Select(s => s.StoreId));
        Assert.Contains(Stores[1].StoreId, stores.Select(s => s.StoreId));
    }
    
    [Fact]
    public async Task TestDeleteStoreInvalidBinding()
    {
        Error error = (await _storeController.Delete(BadId)).Error();
        
        Assert.Equal(NotFoundStatusCode, error.Status);
        
        List<StoreResponse> stores = (await _storeController.All()).Value<List<StoreResponse>>();
        
        Assert.Equal(2, stores.Count);
        Assert.Contains(Stores[0].StoreId, stores.Select(s => s.StoreId));
        Assert.Contains(Stores[1].StoreId, stores.Select(s => s.StoreId));
    }
}