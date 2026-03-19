using System.Net;
using System.Net.Http.Json;
using CartSync.Models;
using CartSyncTests.Core;
using SeedData = CartSync.Models.Seeding.SeedData;

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
}