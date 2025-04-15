namespace DxRating.Services.Email.Options;

public class EmailOptions
{
    public string From { get; set; } = null!;

    public string FromName { get; set; } = null!;

    public string? TemplatePath { get; set; }
}
