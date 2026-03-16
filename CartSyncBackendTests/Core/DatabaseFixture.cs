using CartSyncBackend.Database;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using static CartSyncBackendTests.Core.Constants;

namespace CartSyncBackendTests.Core;

[UsedImplicitly]
public class DatabaseFixture
{
    private static readonly Lock LockObject = new();
    private static bool _databaseInitialized;
    
    public DatabaseFixture()
    {
        lock (LockObject)
        {
            if (_databaseInitialized)
            {
                return;
            }

            using (CartSyncContext context = Context)
            {
                context.Seed();
            }

            _databaseInitialized = true;
        }
    }
    
    public static CartSyncContext Context =>
        new(
            new DbContextOptionsBuilder<CartSyncContext>()
                .UseNpgsql(ConnectionString).Options);
}