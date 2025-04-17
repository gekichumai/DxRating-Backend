using DxRating.Services.Email.Options;
using MailKit.Security;

namespace DxRating.Tests.Services.Email;

public class SmtpConnectionStringTest
{
    [Test]
    public async Task Parse_NoAuth_NoQuery_Smtp()
    {
        const string connectionString = "smtp://localhost:25";

        var parsed = new SmtpOptions(connectionString);

        await Assert.That(parsed.Host).IsEqualTo("localhost");
        await Assert.That(parsed.Port).IsEqualTo(25);
        await Assert.That(parsed.UserName).IsNull();
        await Assert.That(parsed.Password).IsNull();
        await Assert.That(parsed.SecureSocket).IsEqualTo(SecureSocketOptions.None);
    }

    [Test]
    public async Task Parse_Auth_Query_Smtp()
    {
        const string connectionString = "smtp://abc:efg@localhost:25?starttls=true";

        var parsed = new SmtpOptions(connectionString);

        await Assert.That(parsed.Host).IsEqualTo("localhost");
        await Assert.That(parsed.Port).IsEqualTo(25);
        await Assert.That(parsed.UserName).IsEqualTo("abc");
        await Assert.That(parsed.Password).IsEqualTo("efg");
        await Assert.That(parsed.SecureSocket).IsEqualTo(SecureSocketOptions.StartTls);
    }

    [Test]
    public async Task Parse_Auth_NoQuery_Smtps()
    {
        const string connectionString = "smtps://abc:efg@localhost:25";

        var parsed = new SmtpOptions(connectionString);

        await Assert.That(parsed.Host).IsEqualTo("localhost");
        await Assert.That(parsed.Port).IsEqualTo(25);
        await Assert.That(parsed.UserName).IsEqualTo("abc");
        await Assert.That(parsed.Password).IsEqualTo("efg");
        await Assert.That(parsed.SecureSocket).IsEqualTo(SecureSocketOptions.SslOnConnect);
    }

    [Test]
    public async Task Parse_Auth_Query_Smtps()
    {
        const string connectionString = "smtps://abc:efg@localhost:25?starttls=true";

        var parsed = new SmtpOptions(connectionString);

        await Assert.That(parsed.Host).IsEqualTo("localhost");
        await Assert.That(parsed.Port).IsEqualTo(25);
        await Assert.That(parsed.UserName).IsEqualTo("abc");
        await Assert.That(parsed.Password).IsEqualTo("efg");
        await Assert.That(parsed.SecureSocket).IsEqualTo(SecureSocketOptions.StartTls);
    }
}
