using System.Text.Json.Serialization;
using DxRating.Common.Models.Data.Enums;

namespace DxRating.Common.Models.Data;

public record DxType
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(DxSheetTypeJsonConverter))]
    public DxSheetType Type { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("abbr")]
    public string Abbreviation { get; set; } = null!;

    [JsonPropertyName("iconUrl")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("iconHeight")]
    public int? IconHeight { get; set; }
}
