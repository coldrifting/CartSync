using System.Net;
using CartSync.Data.Requests;
using CartSync.SeedData;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class PrepControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestPrepRoutes_RequireAuthorization()
    {
        HttpResponseMessage allPrepsResult = await GetAsyncAnonymous("/api/preps");
        Assert.Equal(HttpStatusCode.Unauthorized, allPrepsResult.StatusCode);

        AddRequest prepAddRequest = new() { Name = "New Prep Name" };
        HttpResponseMessage addPrepResult = await PostAsyncAnonymous("/api/preps/add", prepAddRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, addPrepResult.StatusCode);
        Assert.DoesNotContain(prepAddRequest.Name, Context.Preps.Select(p => p.PrepName));

        Ulid prepId = SeedData.Preps[4].PrepId;
        HttpResponseMessage prepUsagesResult = await GetAsyncAnonymous($"/api/preps/{prepId}/usages");
        Assert.Equal(HttpStatusCode.Unauthorized, prepUsagesResult.StatusCode);
        
        JsonPatchDocument<PrepEditRequest> prepEditRequest = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/PrepName",
                    value = prepAddRequest.Name
                }
            }
        };
        HttpResponseMessage prepEditResult = await PatchAsyncAnonymous($"/api/preps/{prepId}/edit", prepEditRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, prepEditResult.StatusCode);
        Assert.DoesNotContain(prepAddRequest.Name, Context.Preps.Select(p => p.PrepName));
        
        HttpResponseMessage prepDeleteRequest = await DeleteAsyncAnonymous($"/api/preps/{prepId}/delete");
        Assert.Equal(HttpStatusCode.Unauthorized, prepDeleteRequest.StatusCode);
        Assert.Contains(SeedData.Preps[4].PrepName, Context.Preps.Select(p => p.PrepName));
        Assert.Equal(8, Context.Preps.Count());
    }
}