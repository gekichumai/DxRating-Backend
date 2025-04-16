namespace DxRating.Services.Authentication.Models;

public record VerificationTokenDescriptor
{
    public string VerificationToken { get; set; } = null!;

    public DateTimeOffset ExpireAt { get; set; }
}
