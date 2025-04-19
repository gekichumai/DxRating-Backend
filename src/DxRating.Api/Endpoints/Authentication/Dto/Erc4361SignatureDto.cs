using System.Text.Json.Serialization;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record Erc4361SignatureDto
{
    /// <summary>
    /// Erc4361 challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Signature of the message from the wallet.
    /// </summary>
    [JsonPropertyName("signature")]
    public string Signature { get; set; } = string.Empty;
}
