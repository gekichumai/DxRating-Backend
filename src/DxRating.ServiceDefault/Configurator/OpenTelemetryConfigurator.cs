using DxRating.ServiceDefault.Extensions;
using DxRating.ServiceDefault.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace DxRating.ServiceDefault.Configurator;

internal static class OpenTelemetryConfigurator
{
    internal static void ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = false;
            logging.IncludeScopes = true;
        });

        var serviceName = builder.Configuration.GetServiceName();

        builder.Services.AddOpenTelemetry()
            .WithMetrics(meter =>
            {
                meter
                    .AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddAspNetCoreInstrumentation();
            })
            .WithTracing(tracer =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    tracer.SetSampler(new AlwaysOnSampler());
                }

                tracer.AddSource(serviceName);

                var httpClientIgnoreUrls = ((string[])
                [
                    builder.Configuration[TelemetryEnvironment.OtelExporterOtlpEndpoint] ?? string.Empty,
                    builder.Configuration[TelemetryEnvironment.OtelExporterOtlpMetricsEndpoint] ?? string.Empty,
                    builder.Configuration[TelemetryEnvironment.OtelExporterOtlpTracesEndpoint] ?? string.Empty,
                    builder.Configuration[TelemetryEnvironment.OtelExporterOtlpLogsEndpoint] ?? string.Empty,
                ]).Where(x => string.IsNullOrEmpty(x) is false).Distinct().ToArray();

                tracer
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation(http =>
                    {
                        http.FilterHttpRequestMessage = r =>
                        {
                            var requestUri = r.RequestUri?.AbsoluteUri;
                            if (requestUri is null)
                            {
                                return true;
                            }
                            if (httpClientIgnoreUrls.Length == 0)
                            {
                                return true;
                            }

                            // Will return true if any of the urls in httpClientIgnoreUrls starts with the requestUri
                            // Invert the result to ignore the requestUri (return false for ignore)
                            var result = httpClientIgnoreUrls
                                .Any(x => requestUri.StartsWith(x, StringComparison.OrdinalIgnoreCase));

                            return !result;
                        };
                    });

                tracer.AddOtlpExporter();
            });

        builder.AddOpenTelemetryExporters();
    }

    private static void AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var genericOtlpEndpoint = builder.Configuration[TelemetryEnvironment.OtelExporterOtlpEndpoint];
        var meterOtlpEndpoint = builder.Configuration[TelemetryEnvironment.OtelExporterOtlpMetricsEndpoint] ?? genericOtlpEndpoint;
        var tracingOtlpEndpoint = builder.Configuration[TelemetryEnvironment.OtelExporterOtlpTracesEndpoint] ?? genericOtlpEndpoint;

        if (string.IsNullOrEmpty(meterOtlpEndpoint) is false)
        {
            builder.Services.ConfigureOpenTelemetryMeterProvider(metrics =>
            {
                metrics.AddOtlpExporter();
            });
        }

        if (string.IsNullOrEmpty(tracingOtlpEndpoint) is false)
        {
            builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
            {
                tracing.AddOtlpExporter();
            });
        }
    }
}
