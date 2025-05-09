using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DxRating.Domain.Enums;

namespace DxRating.Domain.Entities.Identity;

[Table("token")]
public record Token
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("token_type")]
    public TokenType TokenType { get; set; }

    [Column("verification_token")]
    public string VerificationToken { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }

    // Relations
    public User User { get; set; } = null!;
}
