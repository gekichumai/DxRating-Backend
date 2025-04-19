using System.Text.Json.Serialization;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record RefreshSessionDto
{
    /// <summary>
    /// Current refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;
}
