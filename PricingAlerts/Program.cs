using PricingAlerts.Config;
using PricingAlerts.Email;

var config = new Config();
Console.WriteLine($"BRAPI API Key: {config.BrapiApiKey}");

IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(useMock: true);
emailProvider.SendEmail("user@example.com", "Price Alert", "The price of AAPL dropped below $150.");
