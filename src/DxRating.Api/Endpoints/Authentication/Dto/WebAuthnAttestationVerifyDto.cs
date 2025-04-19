using System.Text.Json.Serialization;
using Fido2NetLib;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record WebAuthnAttestationVerifyDto
{
    /// <summary>
    /// WebAuthn challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Response of the WebAuthn device attestation.
    /// </summary>
    [JsonPropertyName("attestation_response")]
    public AuthenticatorAttestationRawResponse AttestationResponse { get; set; } = null!;
}
