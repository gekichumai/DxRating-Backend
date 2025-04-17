using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DxRating.Domain.Entities.Identity;

[Table("user")]
public record User
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password")]
    public string? Password { get; set; }

    // Email Confirmation
    [Column("email_confirmed")]
    public bool EmailConfirmed { get; set; }

    [Column("email_confirmed_at")]
    public DateTimeOffset EmailConfirmedAt { get; set; }

    // Relations
    public List<SocialLogin> SocialLogins { get; set; } = [];
}
