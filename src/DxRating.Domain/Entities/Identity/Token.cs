using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DxRating.Domain.Enums;

namespace DxRating.Domain.Entities.Identity;

[Table("token")]
public record Token
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }

    [JsonPropertyName("token_type")]
    public TokenType TokenType { get; set; }

    [JsonPropertyName("verification_token")]
    public string VerificationToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }

    // Relations (Key)
    public User User { get; set; } = null!;
}
