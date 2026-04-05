using System.Net;
using CartSync.Models;
using CartSync.Models.Seeding;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class AisleControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestAislesAll_StoreIdInvalid()
    {
        string url = $"api/aisles?storeId={BadIdString}";
        HttpResponseMessage response = await GetAsync(url);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task TestAisleEdit_InvalidPatch()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/StoreName",
                    value = "edited store"
                }
            }
        };
        
        string url = $"api/aisles/{SeedData.Aisles[0].AisleId}/edit?storeId={SeedData.Stores[0].StoreId}";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain("Edited Aisle", Context.Aisles.Select(aisle => aisle.AisleName));
    }
    
    [Fact]
    public async Task TestAisleDelete_BadAisleId()
    {
        Ulid storeId = SeedData.Stores[0].StoreId;
        string url = $"api/aisles/{storeId}/delete?storeId={storeId}";
        HttpResponseMessage response = await DeleteAsync(url.ToLower());
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}