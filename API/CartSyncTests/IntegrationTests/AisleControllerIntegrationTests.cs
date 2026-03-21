using System.Net;
using CartSync.Models;
using CartSync.Models.Seeding;
using CartSyncTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncTests.IntegrationTests;

[Collection("DatabaseTests")]
public class AisleControllerIntegrationTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestGetAislesHttp()
    {
        string url = $"/api/stores/{SeedData.Stores[0].StoreId}/aisles";
        HttpResponseMessage response = await GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task TestGetAislesStoreIdInvalid()
    {
        string url = $"api/stores/{BadIdString}/aisles";
        HttpResponseMessage response = await GetAsync(url);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task TestEditAislePatch()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/AisleName",
                    value = "Edited Aisle"
                }
            }
        };
        
        string url = $"api/stores/{SeedData.Stores[0].StoreId}/aisles/{SeedData.Aisles[0].AisleId}/edit";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Contains("Edited Aisle", Context.Aisles.Select(aisle => aisle.AisleName));
    }
    
    [Fact]
    public async Task TestEditAisleInvalidPatch()
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
        
        string url = $"api/stores/{SeedData.Stores[0].StoreId}/aisles/{SeedData.Aisles[0].AisleId}/edit";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain("Edited Aisle", Context.Aisles.Select(aisle => aisle.AisleName));
    }
    
    [Fact]
    public async Task TestDeleteAisleBadAisleId()
    {
        Ulid storeId = SeedData.Stores[0].StoreId;
        string url = $"api/stores/{storeId}/aisles/{storeId}/delete";
        HttpResponseMessage response = await DeleteAsync(url.ToLower());
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}