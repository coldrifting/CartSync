using CartSyncBackend.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace CartSyncBackendTests.Core;

public class WebAppFixture(WebAppFactory<Program> factory) : IClassFixture<WebAppFactory<Program>>, IAsyncLifetime
{
    protected HttpClient Client = null!;
    protected CartSyncContext Context = null!;
    private IDbContextTransaction _transaction = null!;

    public virtual async Task InitializeAsync()
    {
        Client = factory.CreateClient();
        Context = factory.GetDbContext();
        _transaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
    }
}