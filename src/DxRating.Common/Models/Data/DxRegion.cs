using System.Text.Json.Serialization;

namespace DxRating.Common.Models.Data;

public record DxRegion
{
    [JsonPropertyName("region")]
    public string Region { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
