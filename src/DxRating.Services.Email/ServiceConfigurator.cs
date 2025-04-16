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
        var prefix = $"{assembly.GetName().Name}.Resources.";
        var resources = assembly
            .GetManifestResourceNames()
            .Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            .Where(x => x.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
            .Select(x =>
            {
                var fileName = x[prefix.Length..^5];
                var language = CultureInfoExtensions.ParseLanguage(fileName, true);
                var fileContent = assembly.GetResourceString(x);

                return (language, fileContent);
            })
            .ToDictionary();

        builder.Services.AddKeyedSingleton("Email", new I18N(resources));
        builder.Services.AddSingleton<SmtpClientService>();
        builder.Services.AddSingleton<EmailService>();
    }
}
