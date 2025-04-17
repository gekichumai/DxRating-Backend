using System.Text.Json.Serialization;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record UserTokenDto
{
    /// <summary>
    /// Access token in JWT format, it will have a short lifetime.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Refresh token, a random string, it will have a long lifetime.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// When the access token expires.
    /// </summary>
    [JsonPropertyName("access_token_expires_at")]
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    [JsonPropertyName("refresh_token_expires_at")]
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}
