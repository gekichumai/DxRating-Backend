using System.Text.Json;
using System.Text.Json.Serialization;

namespace DxRating.Worker.Jobs.Models;

public record MaimaiDxAlias
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("content")]
    public List<MaimaiDxAliasContent> Contents { get; set; } = [];
}

public record MaimaiDxAliasContent
{
    [JsonPropertyName("SongID")]
    public int SongId { get; set; }

    [JsonPropertyName("Name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("Alias")]
    public string[] Alias { get; set; } = [];
}

[JsonSerializable(typeof(MaimaiDxAlias))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.General,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
public partial class MaimaiDxAliasJsonContext : JsonSerializerContext;

public static class MaimaiDxAliasJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = MaimaiDxAliasJsonContext.Default
    };

    public static MaimaiDxAlias? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<MaimaiDxAlias>(json, Options);
    }

    private static Dictionary<T, List<string>> MergeAliasList<T>(params Dictionary<T, string[]>[] aliasMaps)
        where T : notnull
    {
        var merged = new Dictionary<T, List<string>>();

        foreach (var aliasMap in aliasMaps)
        {
            foreach (var (k, v) in aliasMap)
            {
                if (merged.ContainsKey(k) is false)
                {
                    merged[k] = [];
                }

                merged[k].AddRange(v);
            }
        }

        return merged;
    }
}
