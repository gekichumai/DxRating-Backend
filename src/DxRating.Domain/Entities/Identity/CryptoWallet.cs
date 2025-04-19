using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DxRating.Domain.Entities.Identity;

[Table("crypto_wallet")]
public record CryptoWallet
{
    [Key]
    [Column("address")]
    public string Address { get; set; } = string.Empty;

    [Column("user_id")]
    public Guid UserId { get; set; }

    // Relations
    public User User { get; set; } = null!;
}
