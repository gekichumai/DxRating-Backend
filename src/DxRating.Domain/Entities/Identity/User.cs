using DxRating.Domain.Entities.Abstract;

namespace DxRating.Domain.Entities.Identity;

public record User : AuditableEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Password { get; set; }

    // Email Confirmation
    public bool EmailConfirmedAt { get; set; }
    public string? EmailConfirmationToken { get; set; }
    public DateTimeOffset? EmailConfirmationSentAt { get; set; }

    // Recovery
    public string? RecoveryToken { get; set; }
    public DateTimeOffset? RecoveryTokenSentAt { get; set; }

    // Email Change
    public string? NewEmail { get; set; }
    public string? EmailChangeToken { get; set; }
    public DateTimeOffset? EmailChangeTokenSentAt { get; set; }
}
