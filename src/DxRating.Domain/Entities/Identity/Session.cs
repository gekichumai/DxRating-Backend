using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace DxRating.Domain.Entities.Identity;

[Table("session")]
public record Session
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [Column("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [Column("access_token_expires_at")]
    public DateTimeOffset AccessTokenExpiresAt { get; set; }

    [Column("refresh_token_expires_at")]
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; } = string.Empty;

    [Column("ip_address")]
    public IPAddress? IpAddress { get; set; }

    // Relations
    public User User { get; set; } = null!;
}
