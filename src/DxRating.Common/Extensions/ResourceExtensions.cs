using System.Reflection;

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
}
