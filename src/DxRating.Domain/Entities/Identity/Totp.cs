using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DxRating.Domain.Entities.Identity;

[Table("totp")]
public record Totp
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("secret")]
    public string Secret { get; set; } = null!;

    [Column("user_id")]
    public Guid UserId { get; set; }

    // Relations
    public User User { get; set; } = null!;
}
