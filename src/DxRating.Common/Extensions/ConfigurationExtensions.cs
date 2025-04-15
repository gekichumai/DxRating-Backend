using Microsoft.Extensions.Configuration;

namespace DxRating.Common.Extensions;

public static class ConfigurationExtensions
{
    public static T GetOptions<T>(this IConfiguration configuration, string section) where T : class, new()
    {
        var options = new T();
        var configurationSection = configuration.GetSection(section);
        configurationSection.Bind(options);

        return options;
    }
}
