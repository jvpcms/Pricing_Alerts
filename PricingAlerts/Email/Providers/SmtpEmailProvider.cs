using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace PricingAlerts.Email.Providers;

public class SmtpEmailProvider : IEmailProvider
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _user;
    private readonly string _password;
    private readonly string _sender;

    public SmtpEmailProvider(string host, int port, string user, string password, string sender)
    {
        _host = host;
        _port = port;
        _user = user;
        _password = password;
        _sender = sender;
    }

    public async Task SendEmail(string destination, string subject, string content)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_sender));
        message.To.Add(MailboxAddress.Parse(destination));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = content };

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_user, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
