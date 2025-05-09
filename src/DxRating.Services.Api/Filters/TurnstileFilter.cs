using System.Text.Json;
using DxRating.Common.Enums;
using DxRating.Common.Extensions;
using DxRating.Services.Api.Extensions;
using DxRating.Services.Api.Models;
using DxRating.Services.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DxRating.Services.Api.Filters;

public class TurnstileFilter : IEndpointFilter
{
    private readonly HttpClient _httpClient;
    private readonly TurnstileOptions _turnstileOptions;
    private readonly ILogger<TurnstileFilter> _logger;

    private static readonly Uri TurnstileBaseAddress = new("https://challenges.cloudflare.com/turnstile/v0/siteverify");

    public TurnstileFilter(HttpClient httpClient, IConfiguration configuration, ILogger<TurnstileFilter> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _turnstileOptions = configuration.GetOptions<TurnstileOptions>("Turnstile");
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_turnstileOptions.Enabled is false)
        {
            return await next(context);
        }

        var metadata = context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<TurnstileMetadata>();
        var action = metadata?.Action;

        var idempotencyKey = Guid.NewGuid();
        context.HttpContext.Request.Headers.TryGetValue("X-DXRating-Turnstile-Response", out var response);
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        var kv = new Dictionary<string, string>
        {
            { "secret", _turnstileOptions.Secret },
            { "response", response.ToString() },
            { "idempotency_key", idempotencyKey.ToString() },
        };
        if (string.IsNullOrEmpty(ip) is false)
        {
            kv.Add("remoteip", ip);
        }

        var body = new FormUrlEncodedContent(kv);

        _logger.LogDebug("Sending Turnstile challenge with idempotency key {IdempotencyKey} and response {Response}", idempotencyKey, response.ToString());
        var challengeResponse = await _httpClient.PostAsync(TurnstileBaseAddress, body);
        if (challengeResponse.IsSuccessStatusCode is false)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, ErrorCode.TurnstileServiceUnavailable);
            return null;
        }

        var responseBody = await challengeResponse.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TurnstileResponse>(responseBody);
        if (result is null)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, ErrorCode.TurnstileServiceUnavailable);
            return null;
        }

        if (result.Success is false)
        {
            var errors = string.Join(", ", result.ErrorCodes);
            _logger.LogWarning("Turnstile challenge failed with idempotency key {IdempotencyKey}, errors {TurnstileError}", idempotencyKey, errors);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, ErrorCode.TurnstileVerificationFailed);
        }

        if (string.IsNullOrEmpty(action) is false && string.IsNullOrEmpty(result.Action) && action != result.Action)
        {
            _logger.LogWarning("Turnstile action mismatch with idempotency key {IdempotencyKey}, expected {ExpectedAction}, actual {ActualAction}", idempotencyKey, action, result.Action);
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, ErrorCode.TurnstileVerificationFailed);
            return null;
        }

        return await next(context);
    }

    private static async Task WriteErrorResponseAsync(EndpointFilterInvocationContext context, int statusCode, ErrorCode code)
    {
        context.HttpContext.Response.StatusCode = statusCode;
        await context.HttpContext.Response.WriteAsJsonAsync(code.ToResponse());
    }
}
