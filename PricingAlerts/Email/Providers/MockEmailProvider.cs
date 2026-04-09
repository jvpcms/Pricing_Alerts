using PricingAlerts.Logging;

namespace PricingAlerts.Email.Providers;

public class MockEmailProvider : IEmailProvider
{
    public Task SendEmail(string destination, string subject, string content)
    {
        Logger.Debug($"[MockEmailProvider] Email sent to '{destination}' | Subject: '{subject}' | Content: '{content}'");
        return Task.CompletedTask;
    }
}
