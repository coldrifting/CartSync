using System.Text;
using System.Text.Json;
using CartSyncBackend.Database;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Storage;

namespace CartSyncBackendTests.Core;

public class AppFixture(AppSetupFactory<Program> setupFactory) : IClassFixture<AppSetupFactory<Program>>, IAsyncLifetime
{
    protected const string BadIdString = "BadId";
    
    protected CartSyncContext Context = null!;
    
    private IDbContextTransaction _transaction = null!;
    private HttpClient _client = null!;

    public virtual Task InitializeAsync()
    {
        Context = setupFactory.GetDbContext();
        _client = setupFactory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        
        _transaction = Context.Database.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _transaction.Rollback();
        return Task.CompletedTask;
    }

    // Workaround for custom URL lowercase rewriter
    protected async Task<HttpResponseMessage> GetAsync(string url, bool lowercase = true) => 
        await _client.GetAsync(lowercase ? url.ToLower() : url);

    protected async Task<HttpResponseMessage> PostAsync(string url, object? obj = null) => 
        await _client.PostAsync(url.ToLower(), ToJsonString(obj));

    protected async Task<HttpResponseMessage> PutAsync(string url, object? obj = null) => 
        await _client.PutAsync(url.ToLower(), ToJsonString(obj));

    protected async Task<HttpResponseMessage> PatchAsync<T>(string url, JsonPatchDocument<T> patchDocument) where T : class =>
        await _client.PatchAsync(url.ToLower(), ToJsonString(patchDocument));

    protected async Task<HttpResponseMessage> DeleteAsync(string url) => 
        await _client.DeleteAsync(url.ToLower());

    private static StringContent ToJsonString(object? obj, bool isJsonPatch = false) => 
        new(JsonSerializer.Serialize(obj),
            Encoding.UTF8, 
            isJsonPatch ? "application/json" : "application/json-patch+json");
}