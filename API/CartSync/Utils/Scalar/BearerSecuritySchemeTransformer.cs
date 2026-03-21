using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace CartSync.Utils.Scalar;

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
    : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(a => a.Name != "Bearer"))
            return;

        OpenApiSecurityScheme bearerScheme = new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        document.Components ??= new OpenApiComponents();
        document.AddComponent("Bearer", bearerScheme);

        OpenApiSecurityRequirement securityRequirement = new()
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        };

        foreach (OpenApiOperation operation in document.Paths.Values
                     .Select(path => path.Operations?.Values)
                     .OfType<Dictionary<HttpMethod, OpenApiOperation>.ValueCollection>()
                     .SelectMany(values => values))
        {
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(securityRequirement);
        }
    }
} 