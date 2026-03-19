using System.Net;
using CartSyncBackend.Models;
using CartSyncBackendTests.Core;

namespace CartSyncBackendTests.IntegrationTests;

[Collection("DatabaseTests")]
public class RecipeControllerIntegrationTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Fact]
    public async Task TestRecipeAdd_EmptyStringShouldError()
    {
        HttpResponseMessage result = await PostAsync("api/recipes/add", new RecipeAddRequest { RecipeName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}