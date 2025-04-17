using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace DxRating.Services.Api.OpenApi;

public class DefaultHeaderTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-DXRating-Api-Version",
            Required = false,
            In = ParameterLocation.Header,
            Description = "API version."
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-DXRating-Language",
            Required = false,
            In = ParameterLocation.Header,
            Description = "Client language. Will override the language set in Accept-Language header."
        });

        return Task.CompletedTask;
    }
}
