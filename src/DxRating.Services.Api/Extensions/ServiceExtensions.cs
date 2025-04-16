using System.Reflection;
using Asp.Versioning;
using DxRating.ServiceDefault.Extensions;
using DxRating.Services.Api.Abstract;
using DxRating.Services.Api.Configurator;
using DxRating.Services.Api.Models;
using DxRating.Services.Api.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace DxRating.Services.Api.Extensions;

public static class ServiceExtensions
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Configuration.GetServiceName();

        builder.ConfigureIdentity();

        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new HeaderApiVersionReader("X-DxRating-Api-Version");
                options.UnsupportedApiVersionStatusCode = StatusCodes.Status400BadRequest;
            })
            .EnableApiVersionBinding();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
            });
        });

        builder.Services.AddOpenApi(serviceName, options =>
        {
            options.AddDocumentTransformer<DefaultApiTransformer>();
            options.AddOperationTransformer<ApiVersionHeaderTransformer>();
            options.AddOperationTransformer<TurnstileHeaderTransformer>();
        });

        builder.Services.AddHttpContextAccessor();
    }

    public static void MapApiEndpoints(this WebApplication app)
    {
        app.MapOpenApiEndpoints();

        app.UseExceptionHandler(builder =>
        {
            builder.Run(async ctx =>
            {
                var exception = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                var exceptionName = exception?.GetType().Name ?? "Unknown";
                var msg = exception?.Message ?? "Unknown exception issue";
                var resp = new ErrorResponse($"{exceptionName}: {msg}");
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(resp);
            });
        });
        app.UseStatusCodePages(async ctx =>
        {
            var code = ctx.HttpContext.Response.StatusCode;
            var msg = new ErrorResponse($"Failed: Get status code {code}");
            await ctx.HttpContext.Response.WriteAsJsonAsync(msg);
        });

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        var api = app.NewVersionedApi();

        if (app.Environment.IsDevelopment() || app.Configuration.GetValue("OpenApi:Enabled", false))
        {
            var svcName = app.Configuration.GetServiceName();
            api.MapGet("/", () => TypedResults.Redirect($"/scalar/{svcName}"))
                .HasApiVersion(1)
                .ExcludeFromDescription();
        }

        var mappers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetInterface(nameof(IEndpointMapper)) is not null)
            .Select(x => x.GetMethod(nameof(IEndpointMapper.MapEndpoints), BindingFlags.Public | BindingFlags.Static));

        var apiGroup = api.MapGroup("/api");

        foreach (var mapper in mappers)
        {
            mapper?.Invoke(null, [apiGroup]);
        }
    }

    private static void MapOpenApiEndpoints(this WebApplication app)
    {
        var enabled = app.Configuration.GetValue("OpenApi:Enabled", false);

        if (app.Environment.IsProduction() && enabled is false)
        {
            return;
        }

        app.MapOpenApi();
        app.MapScalarApiReference();
    }
}
