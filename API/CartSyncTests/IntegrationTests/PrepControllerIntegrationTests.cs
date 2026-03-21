using System.Net;
using System.Net.Http.Json;
using CartSync.Models;
using CartSync.Models.Seeding;
using CartSyncTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace CartSyncTests.IntegrationTests;

[Collection("DatabaseTests")]
public class PrepControllerIntegrationTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestLowercaseRedirect()
    {
        List<PrepResponse> expectedPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        const string url = "/API/PREPS";
        HttpResponseMessage response = await GetAsync(url, false);
        
        Assert.Equal(HttpStatusCode.MovedPermanently, response.StatusCode);

        Assert.NotNull(response.Headers.Location);
        
        HttpResponseMessage redirectedResponse = await GetAsync(response.Headers.Location.AbsolutePath, false);
        Assert.Equal(HttpStatusCode.OK, redirectedResponse.StatusCode);

        List<PrepResponse>? results = await redirectedResponse.Content.ReadFromJsonAsync<List<PrepResponse>>();
        Assert.Equal(expectedPreps, results);
    }
    
    [Fact]
    public async Task TestPrepRoutes_RequireAuthorization()
    {
        HttpResponseMessage allPrepsResult = await GetAsyncAnonymous("/api/preps");
        Assert.Equal(HttpStatusCode.Unauthorized, allPrepsResult.StatusCode);

        PrepAddRequest prepAddRequest = new() { PrepName = "New Prep Name" };
        HttpResponseMessage addPrepResult = await PostAsyncAnonymous("/api/preps/add", prepAddRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, addPrepResult.StatusCode);
        Assert.DoesNotContain(prepAddRequest.PrepName, Context.Preps.Select(p => p.PrepName));

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
                    value = prepAddRequest.PrepName
                }
            }
        };
        HttpResponseMessage prepEditResult = await PatchAsyncAnonymous($"/api/preps/{prepId}/edit", prepEditRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, prepEditResult.StatusCode);
        Assert.DoesNotContain(prepAddRequest.PrepName, Context.Preps.Select(p => p.PrepName));
        
        HttpResponseMessage prepDeleteRequest = await DeleteAsyncAnonymous($"/api/preps/{prepId}/delete");
        Assert.Equal(HttpStatusCode.Unauthorized, prepDeleteRequest.StatusCode);
        Assert.Contains(SeedData.Preps[4].PrepName, Context.Preps.Select(p => p.PrepName));
        Assert.Equal(7, Context.Preps.Count());
    }
}