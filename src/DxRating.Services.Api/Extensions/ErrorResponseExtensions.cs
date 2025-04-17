using DxRating.Common.Enums;
using DxRating.Services.Api.Models;

namespace DxRating.Services.Api.Extensions;

public static class ErrorResponseExtensions
{
    public static ErrorResponse ToResponse(this ErrorCode errorCode, string message = "")
    {
        return new ErrorResponse(errorCode, message);
    }
}
