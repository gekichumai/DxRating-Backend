using DxRating.Services.Api.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace DxRating.Services.Api.OpenApi;

public class TurnstileHeaderTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var endpointMetadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (endpointMetadata.Any(x => x.GetType() == typeof(TurnstileMetadata)))
        {
            operation.Parameters ??= [];

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-DXRating-Turnstile-Response",
                Required = true,
                In = ParameterLocation.Header
            });
        }

        return Task.CompletedTask;
    }
}
