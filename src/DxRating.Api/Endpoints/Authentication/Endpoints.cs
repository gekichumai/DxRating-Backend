using DxRating.Services.Api.Abstract;

namespace DxRating.Api.Endpoints.Authentication;

public partial class Endpoints : IEndpointMapper
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var identityGroup = endpoints.MapGroup("/identity");

        MapLocalAuthenticationEndpoints(identityGroup);
    }
}
