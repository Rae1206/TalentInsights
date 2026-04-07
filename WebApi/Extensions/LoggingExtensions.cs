using Serilog;
using Serilog.Sinks.MongoDB;

namespace WebApi.Extensions;

public static class LoggingExtensions
{
    public static void ConfigureSerilog(this WebApplicationBuilder builder)
    {
        var mongoUrl = builder.Configuration.GetConnectionString("MongoDb");

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "Twitter.WebApi");

            if (!string.IsNullOrEmpty(mongoUrl))
            {
                configuration.WriteTo.MongoDBBson(cfg =>
                {
                    cfg.SetMongoUrl($"{mongoUrl}/TwitterLogs");
                    cfg.SetCollectionName("Logs");
                    cfg.SetCreateCappedCollection(1024);
                    cfg.SetExpireTTL(TimeSpan.FromDays(30));
                });
            }
        });
    }
}
