using System.Net;
using System.Net.Http.Json;
using CartSyncBackend.Models;
using CartSyncBackendTests.Core;
using SeedData = CartSyncBackend.Models.Seeding.SeedData;

namespace CartSyncBackendTests.IntegrationTests;

[Collection("DatabaseTests")]
public class StoreControllerIntegrationTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestAddStoreBadNameEmptyString()
    {
        HttpResponseMessage result = await PostAsync("api/stores/add", new StoreAddRequest { StoreName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task TestHttpDeleteStore()
    {
        string url = $"/api/stores/{SeedData.Stores[0].StoreId}/delete";
        HttpResponseMessage response = await DeleteAsync(url);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        HttpResponseMessage response2 = await DeleteAsync(url);
        
        Assert.Equal(HttpStatusCode.NotFound, response2.StatusCode);
    }
    
    [Fact]
    public async Task TestHttpAddStore()
    {
        const string url = "/api/stores/add";
        HttpResponseMessage response = await PostAsync(url, new StoreAddRequest { StoreName = "New Store Name" });
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Uri? location = response.Headers.Location;
        Assert.NotNull(location);

        Ulid pathId = Ulid.Parse(location.OriginalString.Split('/').Last());

        StoreResponse? value = await response.Content.ReadFromJsonAsync<StoreResponse>();
        Assert.NotNull(value);
        
        Assert.Equal(pathId, value.StoreId);
        
        Assert.Equal(3, Context.Stores.Count());
        Assert.Contains("New Store Name", Context.Stores.Select(s => s.StoreName));
    }
}