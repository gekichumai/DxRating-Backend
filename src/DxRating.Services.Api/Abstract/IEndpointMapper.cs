using Microsoft.AspNetCore.Routing;

namespace DxRating.Services.Api.Abstract;

public interface IEndpointMapper
{
    public static abstract void MapEndpoints(IEndpointRouteBuilder endpoints);
}
