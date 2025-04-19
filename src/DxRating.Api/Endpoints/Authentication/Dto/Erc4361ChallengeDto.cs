using System.Text.Json.Serialization;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record Erc4361ChallengeDto
{
    /// <summary>
    /// Erc4361 challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Erc4361 challenge message that needs to be signed.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
