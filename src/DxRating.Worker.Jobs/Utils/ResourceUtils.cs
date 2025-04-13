using System.Reflection;

namespace DxRating.Worker.Jobs.Utils;

public static class ResourceUtils
{
    public static Stream GetResourceAsync(string fileName)
    {
        var assemblyName = Assembly
            .GetExecutingAssembly()
            .GetName()
            .Name;

        return Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"{assemblyName}.Resources.{fileName}")!;
    }
}
