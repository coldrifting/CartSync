using CartSyncBackend.Controllers;
using CartSyncBackend.Database;

namespace CartSyncBackendTests.Core;

public class DatabaseFixture(DatabaseSetup fixture) : IClassFixture<DatabaseSetup>, IAsyncLifetime
{
    // Prevent xunit warning by using and consuming fixture parameter
    // ReSharper disable once UnusedMember.Global
    protected DatabaseSetup Fixture = fixture;
    
    private CartSyncContext _context = null!;
    protected StoreController StoreController = null!;
    protected AisleController AisleController = null!;
    protected ItemController ItemController = null!;
    protected PrepController PrepController = null!;
    
    /// Start
    public async Task InitializeAsync()
    {
        _context = DatabaseSetup.CreateContext();
        StoreController = new StoreController(_context) { ObjectValidator = new ModelValidator() };
        AisleController = new AisleController(_context) { ObjectValidator = new ModelValidator() };
        ItemController = new ItemController(_context) { ObjectValidator = new ModelValidator() };
        PrepController = new PrepController(_context) { ObjectValidator = new ModelValidator() };
        
        Console.WriteLine("Start");
        await _context.Database.BeginTransactionAsync();
    }

    /// End
    public async Task DisposeAsync()
    {
        Console.WriteLine("End");
        await _context.Database.RollbackTransactionAsync();
    }
}