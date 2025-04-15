using DxRating.ServiceDefault.Configurator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxRating.ServiceDefault.Extensions;

public static class ServicesExtensions
{
    public static void AddServiceDefault(this IHostApplicationBuilder builder)
    {
        builder.ConfigureConfigurations();
        builder.ConfigureOpenTelemetry();
        builder.ConfigureHealthChecks();
        builder.ConfigureSerilog();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        builder.AddRedisDistributedCache("Cache");
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        var healthChecks = app.MapGroup("");

        healthChecks
            .CacheOutput("HealthChecks")
            .WithRequestTimeout("HealthChecks");

        healthChecks.MapHealthChecks("/health");
        healthChecks.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = static r => r.Tags.Contains("live")
        });

        return app;
    }

    private static void ConfigureConfigurations(this IHostApplicationBuilder builder)
    {
        var configurationFile = Path.GetFullPath(
            Environment.GetEnvironmentVariable("DXRATING_CONFIGURATION_FILE") ?? "appsettings.yaml");
        var configurationFileDirectory = Path.GetDirectoryName(configurationFile)!;
        var configurationFileWithoutExt = Path.GetFileNameWithoutExtension(configurationFile);

        var envSpecificFile = Path.Combine(configurationFileDirectory,
            $"{configurationFileWithoutExt}.{builder.Environment.EnvironmentName.ToLowerInvariant()}.yaml");

        if (File.Exists(configurationFile))
        {
            builder.Configuration.AddYamlFile(configurationFile);
        }

        builder.Configuration.AddYamlFile(envSpecificFile, true);
        builder.Configuration.AddEnvironmentVariables();
    }
}
