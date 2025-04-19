using System.Text.Json.Serialization;
using Fido2NetLib;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record WebAuthnAssertionDto
{
    /// <summary>
    /// WebAuthn challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Options that can trigger the assertion.
    /// </summary>
    [JsonPropertyName("options")]
    public AssertionOptions Options { get; set; } = null!;
}
