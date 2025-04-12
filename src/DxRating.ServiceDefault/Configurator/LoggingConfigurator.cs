using System.Globalization;
using DxRating.ServiceDefault.Extensions;
using DxRating.ServiceDefault.Utils;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace DxRating.ServiceDefault.Configurator;

internal static class LoggingConfigurator
{
    internal static void ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(cfg =>
        {
            cfg
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture);

            var otlpEndpoint = builder.Configuration[TelemetryEnvironment.OtelExporterOtlpLogsEndpoint] ??
                               builder.Configuration[TelemetryEnvironment.OtelExporterOtlpEndpoint];
            var otlpProtocolString = builder.Configuration[TelemetryEnvironment.OtelExporterOtlpLogsProtocol] ??
                                     builder.Configuration[TelemetryEnvironment.OtelExporterOtlpProtocol];
            var otlpProtocol = otlpProtocolString switch
            {
                "grpc" => OtlpProtocol.Grpc,
                "http/protobuf" => OtlpProtocol.HttpProtobuf,
                _ => OtlpProtocol.Grpc
            };

            if (string.IsNullOrEmpty(otlpEndpoint))
            {
                return;
            }

            cfg.WriteTo.OpenTelemetry(options =>
            {
                options.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField;
                options.Endpoint = otlpEndpoint;
                options.LogsEndpoint = otlpEndpoint;
                options.Protocol = otlpProtocol;

                var serviceName = builder.Configuration.GetServiceName();
                var headers = (
                        builder.Configuration[TelemetryEnvironment.OtelExporterOtlpLogsHeaders] ??
                        builder.Configuration[TelemetryEnvironment.OtelExporterOtlpHeaders])
                    .ParseAsDictionary();
                var resources = builder.Configuration[TelemetryEnvironment.OtelResourceAttributes].ParseAsDictionary();

                // Headers
                foreach (var (k, v) in headers)
                {
                    options.Headers[k] = v;
                }

                // Resource Attributes
                foreach (var (k, v) in resources)
                {
                    options.ResourceAttributes[k] = v;
                }

                // Service Name
                if (options.ResourceAttributes.ContainsKey("service.name") is false)
                {
                    options.ResourceAttributes["service.name"] = serviceName;
                }
            });
        });
    }
}
