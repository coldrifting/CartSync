using System.Net;
using System.Net.Http.Json;
using CartSync.Models;
using CartSyncTests.Base;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class StoreControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestStoreAdd_BadStoreNameEmptyString()
    {
        HttpResponseMessage result = await PostAsync("api/stores/add", new StoreAddRequest { StoreName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
    
    [Fact]
    public async Task TestStoreAdd_HasLocationHeader()
    {
        const string url = "/api/stores/add";
        HttpResponseMessage response = await PostAsync(url, new StoreAddRequest { StoreName = "New Store Name" });
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Uri? location = response.Headers.Location;
        Assert.NotNull(location);

        Ulid pathId = Ulid.Parse(location.OriginalString.Split('/').Last());

        StoreResponse? value = await response.Content.ReadFromJsonAsync<StoreResponse>(TestContext.Current.CancellationToken);
        Assert.NotNull(value);
        
        Assert.Equal(pathId, value.StoreId);
        
        Assert.Equal(3, (await GetStores()).Count);
        Assert.Equal(3, Context.Stores.Count());
        Assert.Contains("New Store Name", Context.Stores.Select(s => s.StoreName));
        
        // Restore state
        await DeleteAsync(location.OriginalString);
    }

    private async Task<List<StoreResponse>> GetStores()
    {
        HttpResponseMessage response = await GetAsync($"api/stores");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        List<StoreResponse>? values = await response.Content.ReadFromJsonAsync<List<StoreResponse>>();
        Assert.NotNull(values);
        return values;
    }
}