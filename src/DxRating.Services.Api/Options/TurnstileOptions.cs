namespace DxRating.Services.Api.Options;

public record TurnstileOptions
{
    public string Secret { get; set; } = string.Empty;
}
