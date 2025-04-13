using System.Text.Json.Serialization;
using DxRating.Common.Models.Data.Enums;

namespace DxRating.Common.Models.Data;

public record DxVersion
{
    [JsonPropertyName("version")]
    [JsonConverter(typeof(DxVersionTypeJsonConverter))]
    public DxVersionType Version { get; set; } = null!;

    [JsonPropertyName("abbr")]
    public string Abbreviation { get; set; } = null!;
}
