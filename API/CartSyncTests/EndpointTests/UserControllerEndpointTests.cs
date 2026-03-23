using System.Net;
using CartSync.Models;
using CartSyncTests.Base;

namespace CartSyncTests.EndpointTests;

[Collection("DatabaseTests")]
public class UserControllerEndpointTests(AppSetupFactory<Program> setupFactory) : AppFixture(setupFactory)
{
    [Theory]
    [InlineData("test", "test")]
    [InlineData("cartsync", "test")]
    [InlineData("test", "cartsync")]
    public async Task TestLogin_BadUsernameOrPassword(string username, string password)
    {
        UserLoginRequest details = new(username, password);
        HttpResponseMessage loginAttemptResult = await PostAsyncAnonymous("/api/user/login", details);
        Assert.Equal(HttpStatusCode.BadRequest, loginAttemptResult.StatusCode);
    }
}