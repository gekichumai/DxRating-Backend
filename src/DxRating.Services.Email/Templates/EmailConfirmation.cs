namespace DxRating.Services.Email.Templates;

public record EmailConfirmation
{
    public string CallbackUrl { get; set; } = null!;

    public DateTimeOffset ExpireAt { get; set; }
}
