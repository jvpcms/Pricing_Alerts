using PricingAlerts.Config;
using PricingAlerts.Email;
using PricingAlerts.Pricing;

var config = new AppConfig();

IPricingProvider pricingProvider = PricingProviderFactory.GetPricingProvider(useMock: true);
decimal price = await pricingProvider.GetCurrentPrice("PETR4");
Console.WriteLine($"PETR4 current price: R$ {price}");

IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(useMock: true);
await emailProvider.SendEmail("user@example.com", "Price Alert", $"PETR4 is at R$ {price}.");
