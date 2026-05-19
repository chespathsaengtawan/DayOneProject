using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DayOneAPI.Transformers;

public sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type        = SecuritySchemeType.Http,
            Scheme      = "bearer",
            BearerFormat = "JWT",
            Description = "ใส่ JWT token ที่ได้จาก POST /api/auth/login"
        };
        return Task.CompletedTask;
    }
}
