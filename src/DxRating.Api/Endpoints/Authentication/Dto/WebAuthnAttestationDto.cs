using System.Text.Json.Serialization;
using Fido2NetLib;

namespace DxRating.Api.Endpoints.Authentication.Dto;

public record WebAuthnAttestationDto
{
    /// <summary>
    /// WebAuthn challenge id.
    /// </summary>
    [JsonPropertyName("challenge_id")]
    public Guid ChallengeId { get; set; }

    /// <summary>
    /// Options that can trigger the attestation.
    /// </summary>
    [JsonPropertyName("options")]
    public CredentialCreateOptions Options { get; set; } = null!;
}
