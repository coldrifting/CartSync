using System.Text.Json.Serialization;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models;
using CartSyncBackend.Utils.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string conString = builder.Configuration.GetConnectionString("DatabaseContext") ??
                   throw new InvalidOperationException("Connection string 'DatabaseContext' not found.");

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CartSyncContext>((_, options) => options
        .UseNpgsql(conString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .UseSeeding((context, _) =>
        {
            if (context is CartSyncContext cartSyncContext)
            {
                cartSyncContext.Seed();
            }
        }));
}
else
{
    builder.Services.AddDbContext<CartSyncContext>((_, options) => options
        .UseNpgsql(conString));
}

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
            // ReSharper disable once StringLiteralTypo
            schema.Example = "01ARZ3NDEKTSV4RRFFQ69G5FAV";
        }

        return Task.CompletedTask;
    });
});

builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => {
    apiBehaviorOptions.InvalidModelStateResponseFactory = actionContext =>
    {
        Dictionary<string, string?> errors = actionContext.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.AttemptedValue
            );

        return new BadRequestObjectResult(Error.BadRequestModelInvalid(errors).Value);
    };
});

builder.Services.Configure<JsonOptions>(opt =>
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
        .SortTagsAlphabetically()
        .WithClassicLayout()
    );
}


app.MapControllers();

app.Run();