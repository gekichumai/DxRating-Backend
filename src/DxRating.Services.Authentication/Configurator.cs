using DxRating.Services.Authentication.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxRating.Services.Authentication;

public static class Configurator
{
    public static void AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<LocalAuthenticationService>();
    }
}
