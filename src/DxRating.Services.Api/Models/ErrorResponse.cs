using System.Text.Json.Serialization;

namespace DxRating.Services.Api.Models;

public record ErrorResponse
{
    public ErrorResponse(string message)
    {
        Message = message;
    }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
