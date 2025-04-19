using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DxRating.Domain.Entities.Identity;

[Table("webauthn_device")]
public record WebAuthnDevice
{
    [Key]
    [Column("descriptor_id")]
    public byte[] DescriptorId { get; set; } = [];

    [Column("public_key")]
    public byte[] PublicKey { get; set; } = [];

    [Column("user_handle")]
    public byte[] UserHandle { get; set; } = [];

    [Column("aa_guid")]
    public Guid AaGuid { get; set; }

    [Column("signature_counter")]
    public uint SignatureCounter { get; set; }

    [Column("cred_type")]
    public string CredType { get; set; } = string.Empty;

    [Column("user_id")]
    public Guid UserId { get; set; }

    // Relations
    public User User { get; set; } = null!;
}
