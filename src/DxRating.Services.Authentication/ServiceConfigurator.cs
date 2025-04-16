using DxRating.Services.Authentication.Configurator;
using DxRating.Services.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxRating.Services.Authentication;

public static class ServiceConfigurator
{
    public static void ConfigureAuthentication(this IHostApplicationBuilder builder)
    {
        builder.ConfigureIdentity();

        builder.Services.AddScoped<LocalAuthenticationService>();
    }
}
