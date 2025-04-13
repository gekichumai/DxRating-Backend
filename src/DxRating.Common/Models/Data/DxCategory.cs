using System.Text.Json.Serialization;
using DxRating.Common.Models.Data.Enums;

namespace DxRating.Common.Models.Data;

public record DxCategory
{
    [JsonPropertyName("category")]
    [JsonConverter(typeof(DxCategoryTypeJsonConverter))]
    public DxCategoryType Category { get; set; } = null!;
}
