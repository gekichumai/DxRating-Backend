using System.Text.Json.Serialization;
using DxRating.Common.Models.Data.Enums;

namespace DxRating.Common.Models.Data;

public record DxDifficulty
{
    [JsonPropertyName("difficulty")]
    [JsonConverter(typeof(DxDifficultyTypeJsonConverter))]
    public DxDifficultyType Difficulty { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("color")]
    public string Color { get; set; } = null!;
}
