using System.Text.Json.Serialization;
using Fido2NetLib;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record WebAuthnAssertionVerifyDto
{
    /// <summary>
    /// WebAuthn challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Response of the WebAuthn device assertion.
    /// </summary>
    [JsonPropertyName("assertion_response")]
    public AuthenticatorAssertionRawResponse AssertionResponse { get; set; } = null!;
}
