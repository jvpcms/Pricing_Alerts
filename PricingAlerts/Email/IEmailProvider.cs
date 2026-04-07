namespace PricingAlerts.Email;

public interface IEmailProvider
{
    Task SendEmail(string destination, string subject, string content);
}
