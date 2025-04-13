using System.Text.Json;
using System.Text.Json.Serialization;

namespace DxRating.Worker.Jobs.Models;

public record MaimaiOfficialSong
{
    [JsonPropertyName("artist")]
    public string Artist { get; set; } = null!;

    [JsonPropertyName("catcode")]
    public string CategoryCode { get; set; } = null!;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = null!;

    [JsonPropertyName("release")]
    public string Release { get; set; } = null!;

    [JsonPropertyName("lev_bas")]
    public string? LevBas { get; set; }

    [JsonPropertyName("lev_adv")]
    public string? LevAdv { get; set; }

    [JsonPropertyName("lev_exp")]
    public string? LevExp { get; set; }

    [JsonPropertyName("lev_mas")]
    public string? LevMas { get; set; }

    [JsonPropertyName("sort")]
    public string Sort { get; set; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("title_kana")]
    public string TitleKana { get; set; } = null!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = null!;

    [JsonPropertyName("lev_remas")]
    public string? LevRemas { get; set; } = null!;

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("dx_lev_bas")]
    public string? DxLevBas { get; set; }

    [JsonPropertyName("dx_lev_adv")]
    public string? DxLevAdv { get; set; }

    [JsonPropertyName("dx_lev_exp")]
    public string? DxLevExp { get; set; }

    [JsonPropertyName("dx_lev_mas")]
    public string? DxLevMas { get; set; }

    [JsonPropertyName("dx_lev_remas")]
    public string? DxLevRemas { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("lev_utage")]
    public string? LevUtage { get; set; }

    [JsonPropertyName("kanji")]
    public string? Kanji { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    [JsonPropertyName("buddy")]
    public string? Buddy { get; set; }
}

[JsonSerializable(typeof(List<MaimaiOfficialSong>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.General,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
public partial class MaimaiOfficialSongsJsonContext : JsonSerializerContext;

public static class MaimaiOfficialSongsSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = MaimaiOfficialSongsJsonContext.Default
    };

    public static List<MaimaiOfficialSong> Deserialize(string json)
    {
        return JsonSerializer.Deserialize<List<MaimaiOfficialSong>>(json, Options)!;
    }
}
