namespace PricingAlerts.Email.Providers;

public class MockEmailProvider : IEmailProvider
{
    public Task SendEmail(string destination, string subject, string content)
    {
        Console.WriteLine($"[MockEmailProvider] Email sent to '{destination}' | Subject: '{subject}' | Content: '{content}'");
        return Task.CompletedTask;
    }
}
