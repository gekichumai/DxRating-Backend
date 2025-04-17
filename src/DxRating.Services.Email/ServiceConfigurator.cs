using DxRating.Common.Extensions;
using DxRating.Common.Services;
using DxRating.Services.Email.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DxRating.Services.Email;

public static class ServiceConfigurator
{
    public static void ConfigureEmail(this IHostApplicationBuilder builder)
    {
        var assembly = typeof(ServiceConfigurator).Assembly;
        var resources = assembly.GetI18NResources("Resources");

        builder.Services.AddKeyedSingleton("Email", new I18N(resources));
        builder.Services.AddSingleton<SmtpClientService>();
        builder.Services.AddSingleton<EmailService>();
    }
}
