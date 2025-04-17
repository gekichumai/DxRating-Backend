namespace DxRating.Services.Api.Options;

public record TurnstileOptions
{
    public bool Enabled { get; set; }
    public string Secret { get; set; } = string.Empty;
}
