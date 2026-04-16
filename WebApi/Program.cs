using Twitter.Domain.Database.SqlServer;
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

    // Cargar configuración de secret.json
    builder.Configuration.AddJsonFile("secret.json", optional: true, reloadOnChange: true);

    // Configuración infraestructura
    builder.ConfigureSerilog();
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    // TODA la infraestructura consolidada (DbContext, Cache, Repositorios, Servicios, JWT)
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    // Configurar pipeline
    app.ConfigurePipeline();

    // Crear usuario administrador por defecto si no existe
    app.SeedDefaultAdmin();

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