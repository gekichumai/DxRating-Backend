using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxRating.Services.Api.Configurator;

internal static class IdentityConfigurator
{
    internal static void ConfigureIdentity(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();
    }
}
