using PricingAlerts.Config;
using PricingAlerts.Email;
using PricingAlerts.Pricing;
using PricingAlerts.PriceTracker;

if (args.Length != 3
    || !decimal.TryParse(args[1], out var lowPrice)
    || !decimal.TryParse(args[2], out var highPrice))
{
    Console.Error.WriteLine("Usage: stock-quote-alert <ticker> <low_price> <high_price>");
    Console.Error.WriteLine("Example: stock-quote-alert PETR4 22.59 22.67");
    return 1;
}

var ticker = args[0];
var config = new AppConfig();

IPricingProvider pricingProvider = PricingProviderFactory.GetPricingProvider(config);
IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(config);

var tracker = new PriceTracker(
    ticker,
    lowPrice,
    highPrice,
    config.AlertTo,
    emailProvider,
    pricingProvider,
    config.CheckIntervalSeconds);

await tracker.StartAsync();
return 0;
