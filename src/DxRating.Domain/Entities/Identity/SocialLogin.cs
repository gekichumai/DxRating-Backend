using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DxRating.Domain.Entities.Identity;

[Table("social_login")]
public record SocialLogin
{
    [Key]
    [Column("connection_id")]
    public Guid ConnectionId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("platform")]
    public string Platform { get; set; } = string.Empty;

    [Column("identifier")]
    public string Identifier { get; set; } = string.Empty;

    // Relations
    public User User { get; set; } = null!;
}
