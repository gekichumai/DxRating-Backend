using System.Text.Json.Serialization;
using DxRating.Common.Enums;

namespace DxRating.Services.Api.Models;

public record ErrorResponse
{
    public ErrorResponse(ErrorCode errorCode, string message = "")
    {
        ErrorCode = errorCode;
        Message = message;
    }

    [JsonPropertyName("error_code")]
    [JsonConverter(typeof(JsonStringEnumConverter<ErrorCode>))]
    public ErrorCode ErrorCode { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
