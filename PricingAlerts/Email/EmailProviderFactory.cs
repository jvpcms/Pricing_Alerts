using PricingAlerts.Config;
using PricingAlerts.Email.Providers;

namespace PricingAlerts.Email;

public static class EmailProviderFactory
{
    public static IEmailProvider GetEmailProvider(AppConfig config, bool useMock = false)
    {
        if (useMock)
            return new MockEmailProvider();

        return new SmtpEmailProvider(config.SmtpHost, config.SmtpPort, config.SmtpUser, config.SmtpPassword, config.SmtpSender);
    }
}
