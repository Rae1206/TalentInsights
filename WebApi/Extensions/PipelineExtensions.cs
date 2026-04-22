using Scalar.AspNetCore;

namespace WebApi.Extensions;

public static class PipelineExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        // Scalar API Reference (disponible en todos los entornos)
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Twitter API");
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.Theme = ScalarTheme.Purple;
        });

        // OpenAPI para desarrollo
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        // Pipeline de middleware
        app.UseErrorHandler();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Endpoints
        app.MapControllers();
    }
}
