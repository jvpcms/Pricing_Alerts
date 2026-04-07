using PricingAlerts.Config;
using PricingAlerts.Email;
using PricingAlerts.Pricing;
using PricingAlerts.PriceTracker;

var config = new AppConfig();

IPricingProvider pricingProvider = PricingProviderFactory.GetPricingProvider(config, useMock: true);
IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(config, useMock: true);

var tracker = new PriceTracker(
    config.Ticker,
    config.LowPrice,
    config.HighPrice,
    config.AlertTo,
    emailProvider,
    pricingProvider,
    config.CheckIntervalSeconds);

await tracker.StartAsync();
