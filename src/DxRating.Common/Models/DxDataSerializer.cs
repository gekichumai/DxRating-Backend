using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using DxRating.Common.Models.Data;

namespace DxRating.Common.Models;

[JsonSerializable(typeof(DxData))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.General)]
public partial class DxDataSerializerContext : JsonSerializerContext;

public static class DxDataSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        TypeInfoResolver = DxDataSerializerContext.Default
    };

    public static string Serialize(DxData dxData)
    {
        return JsonSerializer.Serialize(dxData, _options);
    }

    public static DxData? Deserialize([StringSyntax("json")] string json)
    {
        return JsonSerializer.Deserialize<DxData>(json, _options);
    }

    public static async Task<DxData?> DeserializeAsync(Stream jsonStream, CancellationToken cancellationToken = new())
    {
        return await JsonSerializer.DeserializeAsync<DxData>(jsonStream, _options, cancellationToken);
    }
}
