using System.Text;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Utils.Scalar;
using CartSync.Utils.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string conString = builder.Configuration.GetConnectionString("DatabaseContext") ??
                   throw new InvalidOperationException("Connection string 'DatabaseContext' not found.");

builder.Services.AddDbContext<CartSyncContext>((_, options) =>
{
    options.UseNpgsql(conString);
        
    if (builder.Environment.IsDevelopment())
    {
        options
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
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback);
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});


string key = builder.Configuration["Authentication:Secret"] 
             ?? throw new InvalidOperationException("Authentication:Secret not found");
SymmetricSecurityKey jwtSigningKey = new(Encoding.UTF8.GetBytes(key));

builder.Services.AddSingleton(new JwtAuthentication(key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = jwtSigningKey
        };
        jwtOptions.Events = JwtEvents.BearerEvents();
        jwtOptions.MapInboundClaims = false;
        
        // Production server sits behind reverse proxy that terminates HTTPS
        jwtOptions.RequireHttpsMetadata = false;
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAuthorization();

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

    opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
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

app.UseCors();

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