using CartSync.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CartSyncTests.Base;

[UsedImplicitly]
public class AppSetupFactory<TProgram> : WebApplicationFactory<TProgram>, IDisposable where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextDescriptor = services.SingleOrDefault(
                serviceDescriptor => serviceDescriptor.ServiceType == typeof(DbContextOptions<CartSyncContext>));

            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            services.AddDbContext<CartSyncContext>((_, options) =>
            {
                DatabaseSetup.GetContextOptions(options);
            });
            
            DatabaseSetup.ResetDatabase();
        });
    }

    public new void Dispose()
    {
        DatabaseSetup.ResetDatabase();
        GC.SuppressFinalize(this);
    }

    public CartSyncContext GetDbContext()
    {
        IServiceScope scope = Services.CreateScope();
        CartSyncContext context = scope.ServiceProvider.GetRequiredService<CartSyncContext>();
        return context;
    }
}