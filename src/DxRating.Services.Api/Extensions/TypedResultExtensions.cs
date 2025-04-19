using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DxRating.Services.Api.Extensions;

public static class TypedResultExtensions
{
    public static Ok<T> ToOk<T>(this T obj) => TypedResults.Ok(obj);

    public static BadRequest<T> ToBadRequest<T>(this T obj) => TypedResults.BadRequest(obj);

    public static NotFound<T> ToNotFound<T>(this T obj) => TypedResults.NotFound(obj);
}
