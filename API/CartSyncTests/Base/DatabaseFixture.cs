using System.Security.Claims;
using CartSync.Controllers;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Models.Seeding;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncTests.Base;

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
    public required RecipeController RecipeController;

    /// Start
    public async Task InitializeAsync()
    {
        Context = DatabaseSetup.CreateContext();
        
        StoreController = AddTestController<StoreController>(Context);
        AisleController = AddTestController<AisleController>(Context);
        ItemController = AddTestController<ItemController>(Context);
        PrepController = AddTestController<PrepController>(Context);
        RecipeController = AddTestController<RecipeController>(Context);
        
        await Context.Database.BeginTransactionAsync();
    }

    /// End
    public async Task DisposeAsync()
    {
        await Context.Database.RollbackTransactionAsync();
    }

    private static T AddTestController<T>(CartSyncContext context) where T : ControllerCore
    {
        T controller = (T)Activator.CreateInstance(typeof(T), context)!;
        controller.ObjectValidator = new ModelValidator();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [
                            new Claim("sub", SeedData.Users[0].Username)
                        ]
                    )
                ),
            }
        };
        
        return controller;
    }
}