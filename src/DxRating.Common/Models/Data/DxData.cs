using System.Text.Json.Serialization;

namespace DxRating.Common.Models.Data;

public record DxData
{
    [JsonPropertyName("songs")]
    public List<DxSong> Songs { get; set; } = [];

    [JsonPropertyName("categories")]
    public List<DxCategory> Categories { get; set; } = [];

    [JsonPropertyName("versions")]
    public List<DxVersion> Versions { get; set; } = [];

    [JsonPropertyName("types")]
    public List<DxType> Types { get; set; } = [];

    [JsonPropertyName("difficulties")]
    public List<DxDifficulty> Difficulties { get; set; } = [];

    [JsonPropertyName("regions")]
    public List<DxRegion> Regions { get; set; } = [];

    [JsonPropertyName("updateTime")]
    public DateTimeOffset UpdateTime { get; set; }
}
