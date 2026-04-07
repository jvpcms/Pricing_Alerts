namespace PricingAlerts.Email;

public interface IEmailProvider
{
    void SendEmail(string destination, string subject, string content);
}
