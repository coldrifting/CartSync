using CartSyncBackend.Database;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartSyncBackendTests.Core;

[UsedImplicitly]
public class WebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private bool _databaseInitialized;

    private IConfiguration Configuration { get; set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CartSyncContext>>();
            services.AddDbContext<CartSyncContext>(CartSyncContextBuilderOptions);
        });
        
        builder.ConfigureAppConfiguration((_, config) =>
        {
            Configuration = config.Build(); // Assign the built configuration to a public property
        });
        
        builder.UseEnvironment("Development");
    }
    
    public CartSyncContext GetDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        CartSyncContext context = scope.ServiceProvider.GetRequiredService<CartSyncContext>();

        if (!_databaseInitialized)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            _databaseInitialized = true;
        }
        
        return context;
    }

    private void CartSyncContextBuilderOptions(DbContextOptionsBuilder builder)
    {
        builder
            .UseNpgsql(GetDatabaseConnectionString())
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

    private string GetDatabaseConnectionString()
    {
        // Now you can access the connection string using the IConfiguration property
        // The "DefaultConnection" key is just an example.
        string connectionString = Configuration.GetConnectionString("DatabaseContext") ?? 
                                  throw new InvalidOperationException("DatabaseContext string not found.");
        return connectionString;
    }
}