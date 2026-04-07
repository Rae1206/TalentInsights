using Application;
using Domain;
using Serilog;
using WebApi.Extensions;

// Logger de arranque para errores durante el inicio
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando Twitter Web API");

    var builder = WebApplication.CreateBuilder(args);

    // Configuración infraestructura
    builder.ConfigureSerilog();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Configurar pipeline
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó de forma inesperada");
}
finally
{
    Log.CloseAndFlush();
}
