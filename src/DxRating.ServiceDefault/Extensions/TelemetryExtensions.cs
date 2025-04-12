using DxRating.ServiceDefault.Utils;
using Microsoft.Extensions.Configuration;

namespace DxRating.ServiceDefault.Extensions;

public static class TelemetryExtensions
{
    public static string GetServiceName(this IConfiguration configuration)
    {
        return configuration[TelemetryEnvironment.OtelServiceName] ?? TelemetryEnvironment.DefaultServiceName;
    }
}
