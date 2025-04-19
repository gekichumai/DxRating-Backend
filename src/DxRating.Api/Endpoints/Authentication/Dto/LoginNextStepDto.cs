using System.Text.Json.Serialization;
using DxRating.Api.Endpoints.Authentication.Enums;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record LoginNextStepDto
{
    /// <summary>
    /// What to do next.
    /// </summary>
    [JsonPropertyName("next_step")]
    public LoginNextStep NextStep { get; set; }
}
