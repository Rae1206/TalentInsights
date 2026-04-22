using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;

namespace WebApi.Extensions;

public static class PipelineExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        // Pipeline de middleware primero
        app.UseErrorHandler();
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Registrar controllers
        app.MapControllers();

        // OpenAPI (genera el documento)
        app.MapOpenApi();

        // Scalar UI
        app.MapScalarApiReference();
    }
}