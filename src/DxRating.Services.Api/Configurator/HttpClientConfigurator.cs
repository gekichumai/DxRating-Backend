using System.Text.Json;
using DxRating.Services.Api.Filters;
using DxRating.Services.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace DxRating.Services.Api.Configurator;

public static class HttpClientConfigurator
{
    public static void ConfigureHttpClient(this HostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<TurnstileFilter>()
            .AddResilienceHandler(nameof(TurnstileFilter), x =>
            {
                x.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant,
                    ShouldHandle = async arg =>
                    {
                        if (arg.Outcome.Exception is not null || arg.Outcome.Result is null)
                        {
                            return true;
                        }

                        if (arg.Outcome.Result.IsSuccessStatusCode is false)
                        {
                            return true;
                        }

                        var body = await arg.Outcome.Result.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<TurnstileResponse>(body);
                        if (result is null)
                        {
                            return true;
                        }

                        if (result.Success is false)
                        {
                            return result.ErrorCodes.Contains("internal-error");
                        }

                        return false;
                    }
                });
            });
    }
}
