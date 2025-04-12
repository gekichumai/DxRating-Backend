namespace DxRating.ServiceDefault.Utils;

public static class TelemetryEnvironment
{
    public const string DefaultServiceName = "DxRating";

    public const string OtelServiceName = "OTEL_SERVICE_NAME";
    public const string OtelResourceAttributes = "OTEL_RESOURCE_ATTRIBUTES";

    public const string OtelExporterOtlpEndpoint ="OTEL_EXPORTER_OTLP_ENDPOINT";
    public const string OtelExporterOtlpMetricsEndpoint ="OTEL_EXPORTER_OTLP_METRICS_ENDPOINT";
    public const string OtelExporterOtlpTracesEndpoint ="OTEL_EXPORTER_OTLP_TRACES_ENDPOINT";
    public const string OtelExporterOtlpLogsEndpoint ="OTEL_EXPORTER_OTLP_LOGS_ENDPOINT";

    public const string OtelExporterOtlpProtocol = "OTEL_EXPORTER_OTLP_PROTOCOL";
    public const string OtelExporterOtlpHeaders = "OTEL_EXPORTER_OTLP_HEADERS";

    public const string OtelExporterOtlpLogsProtocol = "OTEL_EXPORTER_OTLP_LOGS_PROTOCOL";
    public const string OtelExporterOtlpLogsHeaders = "OTEL_EXPORTER_OTLP_LOGS_HEADERS";
}
