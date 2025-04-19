namespace DxRating.Services.Authentication.Models;

public record SessionTokenDescriptor
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;

    public DateTimeOffset AccessTokenExpireAt { get; set; }
    public DateTimeOffset RefreshTokenExpireAt { get; set; }
}
