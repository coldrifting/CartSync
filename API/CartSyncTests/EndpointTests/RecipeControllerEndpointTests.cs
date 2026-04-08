using System.Net;
using CartSync.Data.Requests;
using CartSyncTests.Base;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class RecipeControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestRecipeAdd_EmptyStringShouldError()
    {
        HttpResponseMessage result = await PostAsync("api/recipes/add", new AddRequest { Name = "" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}