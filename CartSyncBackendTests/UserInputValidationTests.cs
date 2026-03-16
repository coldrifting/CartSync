using System.Net;
using CartSyncBackendTests.Core;

using static CartSyncBackendTests.Core.Constants;

namespace CartSyncBackendTests;

[Collection("DatabaseUnitTests")]
public class UserInputValidationTests(WebAppFactory<Program> factory) : IClassFixture<WebAppFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    
    [Fact]
    public async Task TestEditStoreInvalidId()
    {
        HttpResponseMessage response = await _client.PutAsync($"api/stores/edit?storeId={BadIdString}&storeName=new store name", null);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestDeleteStoreInvalidId()
    {
        HttpResponseMessage response = await _client.DeleteAsync($"api/stores/delete?storeId={BadIdString}");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}