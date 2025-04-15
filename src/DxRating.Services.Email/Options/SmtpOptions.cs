using MailKit.Security;

namespace DxRating.Services.Email.Options;

public record SmtpOptions
{
    public SmtpOptions(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        // smtp[s]://[username][:password]@<host>[:<port>][?starttls=true|false]
        var uri = new Uri(connectionString);

        Host = uri.Host;
        Port = uri.Port;

        var queries = uri.Query.Length > 1
            ? uri.Query[1..].Split('&').Where(x => x.Length != 0).ToArray()
            : [];

        foreach (var query in queries)
        {
            var parts = query.Split('=');
            if (parts.Length != 2)
            {
                continue;
            }

            var key = parts[0].ToLowerInvariant();
            var value = parts[1].ToLowerInvariant();

            switch (key)
            {
                case "starttls":
                    SecureSocket = value == "true" ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                    break;
            }
        }

        if (uri.Scheme == "smtps")
        {
            if (SecureSocket == SecureSocketOptions.None)
            {
                SecureSocket = SecureSocketOptions.SslOnConnect;
            }
        }

        var userInfo = uri.UserInfo.Split(':');
        if (userInfo.Length != 2)
        {
            UserName = null;
            Password = null;
        }

        UserName = userInfo[0];
        Password = userInfo[1];
    }

    public string Host { get; } = "localhost";
    public int Port { get; } = 25;
    public SecureSocketOptions SecureSocket { get; } = SecureSocketOptions.None;
    public string? UserName { get; }
    public string? Password { get; }
}
