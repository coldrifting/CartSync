using CartSyncBackend.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace CartSyncBackendTests.Core;

public class DatabaseControllerFixture(DatabaseFixture fixture) : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private IDbContextTransaction _transaction = null!;
    protected CartSyncContext Context = null!;

    public virtual async Task InitializeAsync()
    {
        Context = DatabaseFixture.Context;
        _transaction = await Context.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await _transaction.RollbackAsync();
    }
}