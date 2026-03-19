using CartSyncBackend.Controllers;
using CartSyncBackend.Models;

namespace CartSyncBackendTests.Core;

public class DatabaseFixture(DatabaseSetup fixture) : IClassFixture<DatabaseSetup>, IAsyncLifetime
{
    // Prevent xunit warning by using and consuming fixture parameter
    // ReSharper disable once UnusedMember.Global
    protected DatabaseSetup Fixture = fixture;
    
    public required CartSyncContext Context;
    public required StoreController StoreController;
    public required AisleController AisleController;
    public required ItemController ItemController;
    public required PrepController PrepController;
    
    /// Start
    public async Task InitializeAsync()
    {
        Context = DatabaseSetup.CreateContext();
        StoreController = new StoreController(Context) { ObjectValidator = new ModelValidator() };
        AisleController = new AisleController(Context) { ObjectValidator = new ModelValidator() };
        ItemController = new ItemController(Context) { ObjectValidator = new ModelValidator() };
        PrepController = new PrepController(Context) { ObjectValidator = new ModelValidator() };
        
        await Context.Database.BeginTransactionAsync();
    }

    /// End
    public async Task DisposeAsync()
    {
        await Context.Database.RollbackTransactionAsync();
    }
}