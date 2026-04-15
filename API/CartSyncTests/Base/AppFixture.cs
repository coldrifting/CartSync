using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.SeedData;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CartSyncTests.Base;

public class AppFixture(AppSetupFactory<Program> setupFactory) : IClassFixture<AppSetupFactory<Program>>, IAsyncLifetime
{
    protected const string BadIdString = "BadId";
    
    public required CartSyncContext Context;
    private HttpClient _client = null!;
    private HttpClient _clientAnonymous = null!;

    public async ValueTask InitializeAsync()
    {
        WebApplicationFactoryClientOptions clientOptions = new()
        {
            AllowAutoRedirect = false
        };

        Context = setupFactory.GetDbContext();
        _client = setupFactory.CreateClient(clientOptions);
        _clientAnonymous = setupFactory.CreateClient(clientOptions);
            
        await InitializeAuthorizedMethods();
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
    
    // Workaround for custom URL lowercase rewriter
    protected async Task<HttpResponseMessage> GetAsync(string url, bool lowercase = true) => 
        await _client.GetAsync(lowercase ? url.ToLower() : url);

    protected async Task<HttpResponseMessage> PostAsync(string url, object? obj = null) => 
        await _client.PostAsync(url.ToLower(), ToJsonString(obj));

    protected async Task<HttpResponseMessage> PatchAsync<T>(string url, JsonPatchDocument<T> patchDocument) where T : class =>
        await _client.PatchAsync(url.ToLower(), ToJsonString(patchDocument));

    protected async Task<HttpResponseMessage> DeleteAsync(string url) => 
        await _client.DeleteAsync(url.ToLower());
    
    protected async Task<HttpResponseMessage> GetAsyncAnonymous(string url, bool lowercase = true) => 
        await _clientAnonymous.GetAsync(lowercase ? url.ToLower() : url);

    protected async Task<HttpResponseMessage> PostAsyncAnonymous(string url, object? obj = null) => 
        await _clientAnonymous.PostAsync(url.ToLower(), ToJsonString(obj));

    protected async Task<HttpResponseMessage> PatchAsyncAnonymous<T>(string url, JsonPatchDocument<T> patchDocument) where T : class =>
        await _clientAnonymous.PatchAsync(url.ToLower(), ToJsonString(patchDocument));

    protected async Task<HttpResponseMessage> DeleteAsyncAnonymous(string url) => 
        await _clientAnonymous.DeleteAsync(url.ToLower());

    private static StringContent ToJsonString(object? obj, bool isJsonPatch = false) => 
        new(JsonSerializer.Serialize(obj),
            Encoding.UTF8, 
            isJsonPatch ? "application/json" : "application/json-patch+json");

    private async Task InitializeAuthorizedMethods()
    {
        string testUser = SeedData.Users[0].Username;
        HttpResponseMessage result = await _client.PostAsJsonAsync("/api/user/login/token", 
            new UserLoginRequest(testUser, testUser));
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        UserLoginSuccessResponse? resultContent = await result.Content.ReadFromJsonAsync<UserLoginSuccessResponse>();
        Assert.NotNull(resultContent);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", resultContent.Token);
    }
}