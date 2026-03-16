using CartSyncBackend.Database;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CartSyncBackendTests.Core;

using static Constants;

[UsedImplicitly]
public class WebAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CartSyncContext>>();

            services.AddDbContext<CartSyncContext>(options =>
            {
                options
                    .UseNpgsql(ConnectionString)
                    .UseSeeding((context, _) =>
                    {
                        if (context is CartSyncContext cartSyncContext)
                        {
                            cartSyncContext.Seed();
                        }
                    });
            });
        });
        
        builder.UseEnvironment("Development");
    }
}