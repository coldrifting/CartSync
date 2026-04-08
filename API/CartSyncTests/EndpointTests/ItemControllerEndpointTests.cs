using System.Net;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Objects;
using CartSync.SeedData;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class ItemControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestItemEdit()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/Temp",
                    value = Temp.Frozen
                }
            }
        };
        
        string url = $"api/items/{SeedData.Items[0].ItemId}/edit";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Item? item = await Context.Items.FindAsync([SeedData.Items[0].ItemId], TestContext.Current.CancellationToken);
        Assert.NotNull(item);
        Assert.Equal(Temp.Frozen, item.Temp);
    }
    
    [Fact]
    public async Task TestItemEdit_String()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/Temp",
                    value = "Cold"
                }
            }
        };
        
        string url = $"api/items/{SeedData.Items[0].ItemId}/edit";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Item? item = await Context.Items.FindAsync([SeedData.Items[0].ItemId], TestContext.Current.CancellationToken);
        Assert.NotNull(item);
        Assert.Equal(Temp.Cold, item.Temp);
    }
    
    [Fact]
    public async Task TestItemEdit_InvalidPatch_ShouldError()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/Temp",
                    value = "InvalidTemp"
                }
            }
        };
        
        string url = $"api/items/{SeedData.Items[0].ItemId}/edit";
        HttpResponseMessage response = await PatchAsync(url, jsonPatch);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Item? item = await Context.Items.FindAsync([SeedData.Items[0].ItemId], TestContext.Current.CancellationToken);
        Assert.NotNull(item);
        Assert.Equal(SeedData.Items[0].Temp, item.Temp);
    }
}