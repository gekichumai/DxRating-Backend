using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using DxRating.Common.Extensions;
using DxRating.Common.Services;
using DxRating.Services.Email.Enums;
using DxRating.Services.Email.Options;
using Fluid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DxRating.Services.Email.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClientService _smtpClientService;
    private readonly I18N _i18N;
    private readonly EmailOptions _emailOptions;

    private static readonly Assembly Assembly = typeof(EmailService).Assembly;
    private static readonly FluidParser FluidParser = new();

    private readonly ConcurrentDictionary<string, IFluidTemplate> _templateCache = [];

    public EmailService(
        IConfiguration configuration,
        SmtpClientService smtpClientService,
        [FromKeyedServices("Email")] I18N i18N,
        ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpClientService = smtpClientService;
        _i18N = i18N;

        _emailOptions = configuration.GetOptions<EmailOptions>("Email");
    }

    public async Task SendAsync(string email, EmailKind kind, object model, CultureInfo cultureInfo)
    {
        var msg = await BuildMimeMessage(email, kind, model, cultureInfo);
        await _smtpClientService.SendAsync(msg);
    }

    private async Task<MimeMessage> BuildMimeMessage(string email, EmailKind kind, object model, CultureInfo cultureInfo)
    {
        var template = await GetTemplateAsync(kind, cultureInfo);

        var html = await template.RenderAsync(new TemplateContext(model));
        var subject = _i18N.Get(kind.ToString(), cultureInfo);

        if (string.IsNullOrEmpty(html) || string.IsNullOrEmpty(subject) || subject == kind.ToString())
        {
            throw new InvalidOperationException($"Template or subject for {kind} is null or empty.");
        }

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = html
        };

        var msg = new MimeMessage();

        msg.From.Add(new MailboxAddress(_emailOptions.FromName, _emailOptions.From));
        msg.To.Add(new MailboxAddress(string.Empty, email));
        msg.Subject = subject;
        msg.Body = bodyBuilder.ToMessageBody();

        return msg;
    }

    private async Task<IFluidTemplate> GetTemplateAsync(EmailKind kind, CultureInfo cultureInfo)
    {
        var key = $"{kind}.{cultureInfo.ParseLanguage().ToLocaleString()}";
        if (_templateCache.TryGetValue(key, out var value))
        {
            return value;
        }

        string? template = null;
        if (Directory.Exists(_emailOptions.TemplatePath))
        {
            var file = Path.Combine(_emailOptions.TemplatePath, $"{key}.liquid");
            if (File.Exists(file))
            {
                template = await File.ReadAllTextAsync(file);
            }
        }

        if (string.IsNullOrEmpty(template))
        {
            template = await Assembly.GetResourceStringAsync($"Templates.{key}.cs.liquid");
        }

        FluidParser.TryParse(template, out var fluidTemplate, out var error);

        if (error is not null || fluidTemplate is null)
        {
            throw new InvalidOperationException(error);
        }

        _templateCache[key] = fluidTemplate;

        return fluidTemplate;
    }
}
