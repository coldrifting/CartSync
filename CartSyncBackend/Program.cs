using System.Text.Json.Serialization;
using CartSyncBackend;
using CartSyncBackend.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CartSyncContext>(optionsBuilder 
    => optionsBuilder
        .UseSqlite(CartSyncContext.DefaultPath)
        .UseSeeding((context, _) =>
        {
            if (context is CartSyncContext cartSyncContext)
            {
                cartSyncContext.Seed();
            }
        })
    );

builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(opt =>
{
    opt.AddSchemaTransformer((schema, context, _) =>
    {
        if (context.JsonTypeInfo.Type == typeof(Ulid))
        {
            schema.Type = JsonSchemaType.String;
            schema.Example = "01ARZ3NDEKTSV4RRFFQ69G5FAV";
        }

        return Task.CompletedTask;
    });
});

builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => {
    apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(opt =>
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

WebApplication app = builder.Build();

// enforce lowercase URLs
// by redirecting uppercase urls to lowercase urls
app.UseRewriter(new RewriteOptions().Add(new RedirectLowerCaseRule()));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api", opt => opt
        .WithTheme(ScalarTheme.Solarized)
        //.WithClassicLayout()
    );
}


app.MapControllers();

app.Run();