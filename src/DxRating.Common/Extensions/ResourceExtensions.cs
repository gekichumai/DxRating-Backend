using System.Reflection;
using DxRating.Common.Enums;

namespace DxRating.Common.Extensions;

public static class ResourceExtensions
{
    public static Stream GetResourceStream(this Assembly assembly, string resourceName)
    {
        var fullPath = $"{assembly.GetName().Name}.{resourceName}";

        var resourcePath = assembly.GetManifestResourceNames().FirstOrDefault(name => name == fullPath)
                           ?? throw new FileNotFoundException($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");

        return assembly.GetManifestResourceStream(resourcePath)
               ?? throw new FileNotFoundException($"Failed to load resource stream for '{resourceName}'.");
    }

    public static string GetResourceString(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static async Task<string> GetResourceStringAsync(this Assembly assembly, string resourceName)
    {
        await using var stream = assembly.GetResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public static Dictionary<Language, string> GetI18NResources(this Assembly assembly, string directoryPath)
    {
        var prefix = $"{assembly.GetName().Name}.{directoryPath}.";
        var resources = assembly
            .GetManifestResourceNames()
            .Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            .Where(x => x.EndsWith(".json", StringComparison.InvariantCultureIgnoreCase))
            .Select(x =>
            {
                var fileName = x[prefix.Length..^5];
                var language = CultureInfoExtensions.ParseLanguage(fileName, true);
                var fileContent = assembly.GetResourceString($"{directoryPath}.{fileName}.json");

                return (language, fileContent);
            })
            .ToDictionary();

        return resources;
    }
}
