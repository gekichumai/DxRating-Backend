using DxRating.Services.Email.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Polly;
using Polly.Retry;

namespace DxRating.Services.Email.Services;

public class SmtpClientService : IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpClientService> _logger;

    private SmtpClient? _smtpClient;
    private readonly ResiliencePipeline _retryPolicy;

    public SmtpClientService(IConfiguration configuration, ILogger<SmtpClientService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _retryPolicy = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Constant
            })
            .Build();
    }

    public async Task SendAsync(MimeMessage message)
    {
        await _retryPolicy.ExecuteAsync(async token =>
        {
            await InitializeSmtpClientAsync();
            await _smtpClient!.SendAsync(message, token);
        });
    }

    private async Task InitializeSmtpClientAsync()
    {
        if (_smtpClient?.IsConnected is true)
        {
            return;
        }

        var options = new SmtpOptions(_configuration.GetConnectionString("Smtp"));

        _smtpClient ??= new SmtpClient();

        await _smtpClient.ConnectAsync(
            host: options.Host,
            port: options.Port,
            options: options.SecureSocket);

        if (string.IsNullOrEmpty(options.UserName) is false &&
            string.IsNullOrEmpty(options.Password))
        {
            await _smtpClient.AuthenticateAsync(
                userName: options.UserName,
                password: options.Password);
        }

        _logger.LogInformation("Connected to SMTP server {Host}:{Port}", options.Host, options.Port);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _smtpClient?.Disconnect(true);
        _smtpClient?.Dispose();

        GC.ReRegisterForFinalize(this);
    }
}
