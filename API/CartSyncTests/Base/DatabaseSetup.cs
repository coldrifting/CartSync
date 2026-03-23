using CartSync.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CartSyncTests.Base;

[UsedImplicitly]
public class DatabaseSetup : IDisposable
{
    private const string ConnectionString =
        "Host=localhost;Username=coldrifting;Database=CartSyncTestDb"; // ;Include Error Detail=true";

    private static readonly Lock Lock = new();
    private static bool _databaseInitialized;

    /// Begin
    public DatabaseSetup()
    {
        lock (Lock)
        {
            if (_databaseInitialized)
            {
                return;
            }

            ResetDatabase();
            _databaseInitialized = true;
        }
    }

    /// Finalize
    public void Dispose()
    {
        // Reset Database
        ResetDatabase();
        GC.SuppressFinalize(this);
    }

    public static void ResetDatabase()
    {
        using CartSyncContext context = CreateContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    public static CartSyncContext CreateContext()
    {
        return new CartSyncContext(GetContextOptions().Options);
    }

    public static DbContextOptionsBuilder GetContextOptions(DbContextOptionsBuilder? options = null) =>
        options ?? new DbContextOptionsBuilder()
            .UseNpgsql(ConnectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseSeeding((context, _) =>
            {
                if (context is CartSyncContext cartSyncContext)
                {
                    cartSyncContext.Seed();
                }
            });
}