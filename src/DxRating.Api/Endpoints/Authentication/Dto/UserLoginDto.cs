using System.Text.Json.Serialization;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record UserLoginDto
{
    /// <summary>
    /// User email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User password.
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}
