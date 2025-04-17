using System.Globalization;
using System.Reflection;
using Asp.Versioning;
using DxRating.Common.Enums;
using DxRating.Database;
using DxRating.ServiceDefault.Extensions;
using DxRating.Services.Api.Abstract;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.OpenApi;
using DxRating.Services.Api.Services;
using DxRating.Services.Authentication;
using DxRating.Services.Authentication.Abstract;
using DxRating.Services.Email;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace DxRating.Services.Api;

public static class ServiceConfigurator
{
    public static void AddApiServices(this IHostApplicationBuilder builder)
    {
        var serviceName = builder.Configuration.GetServiceName();

        builder.ConfigureNpgsql();
        builder.ConfigureAuthentication();
        builder.ConfigureEmail();

        builder.Services.AddScoped<ICurrentUser, CurrentUser>();

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
            options.AddOperationTransformer<DefaultHeaderTransformer>();
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
                var resp = ErrorCode.Unknown.ToResponse($"{exceptionName}: {msg}");
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await ctx.Response.WriteAsJsonAsync(resp);
            });
        });
        app.UseStatusCodePages(async ctx =>
        {
            var code = ctx.HttpContext.Response.StatusCode;
            var msg = ErrorCode.Unknown.ToResponse($"Failed: Get status code {code}");
            await ctx.HttpContext.Response.WriteAsJsonAsync(msg);
        });

        app.UseCors();

        var supportedCultures = new List<CultureInfo>
        {
            new("en"),
            new("ja"),
            new("zh-Hans"),
            new("zh-Hant")
        };

        app.UseRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders =
            [
                new CustomRequestCultureProvider(ctx =>
                {
                    var language = ctx.Request.Headers.TryGetValue("X-DXRating-Language", out var lang)
                        ? lang.ToString()
                        : null;

                    return Task.FromResult(language is null ? null : new ProviderCultureResult(language));
                }),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

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
