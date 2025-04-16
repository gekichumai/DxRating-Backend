namespace DxRating.Services.Authentication.Options;

public record TokenLifetimeOptions
{
    public int EmailConfirmation { get; set; } = 300;

    public int PasswordReset { get; set; } = 300;
}
