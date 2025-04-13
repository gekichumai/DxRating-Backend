using System.Text.Json;
using System.Text.Json.Serialization;

namespace DxRating.Worker.Jobs.Models;

public record MaimaiLxnsNetAlias
{
    [JsonPropertyName("aliases")]
    public List<MaimaiLxnsNetAliasContent> Aliases { get; set; } = [];
}

public record MaimaiLxnsNetAliasContent
{
    [JsonPropertyName("song_id")]
    public int SongId { get; set; }

    [JsonPropertyName("aliases")]
    public List<string> Aliases { get; set; } = [];
}

[JsonSerializable(typeof(MaimaiLxnsNetAlias))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.General,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
public partial class MaimaiLxnsNetAliasJsonContext : JsonSerializerContext;

public static class MaimaiLxnsNetAliasJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = MaimaiLxnsNetAliasJsonContext.Default
    };

    public static MaimaiLxnsNetAlias? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<MaimaiLxnsNetAlias>(json, Options);
    }
}
