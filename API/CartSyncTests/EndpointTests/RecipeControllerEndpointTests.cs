using System.Net;
using CartSync.Models;
using CartSyncTests.Base;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class RecipeControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestRecipeAdd_EmptyStringShouldError()
    {
        HttpResponseMessage result = await PostAsync("api/recipes/add", new RecipeAddRequest { RecipeName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}