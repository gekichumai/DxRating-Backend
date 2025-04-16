namespace DxRating.Services.Authentication.Models;

public record SessionTokenDescriptor
{
    public string AccessToken { get; set; } = null!;

    public DateTimeOffset AccessTokenExpireAt { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTimeOffset RefreshTokenExpireAt { get; set; }
}
