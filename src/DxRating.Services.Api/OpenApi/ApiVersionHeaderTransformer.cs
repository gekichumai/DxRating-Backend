using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace DxRating.Services.Api.OpenApi;

public class ApiVersionHeaderTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-DxRating-Api-Version",
            Required = false,
            In = ParameterLocation.Header
        });

        return Task.CompletedTask;
    }
}
