namespace PricingAlerts.Email;

public class MockEmailProvider : IEmailProvider
{
    public void SendEmail(string destination, string subject, string content)
    {
        Console.WriteLine($"[MockEmailProvider] Email sent to '{destination}' | Subject: '{subject}' | Content: '{content}'");
    }
}
