using PricingAlerts;
using PricingAlerts.Config;
using PricingAlerts.Email;
using PricingAlerts.Pricing;
using PricingAlerts.PriceTracker;

var config = new AppConfig();

if (args.Length == 3
    && decimal.TryParse(args[1], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var lowPrice)
    && decimal.TryParse(args[2], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var highPrice))
{
    var ticker = args[0];

    IPricingProvider pricingProvider = PricingProviderFactory.GetPricingProvider(config);
    IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(config);

    var list = new CyclicBucketList(bucketCount: 1, config.CheckIntervalSeconds, config.MaxBuckets);
    list.AddTracker(new PriceTracker(ticker, lowPrice, highPrice, config.AlertTo, emailProvider, pricingProvider));

    await Task.WhenAll(list.Nodes().Select(n => n.Value.StartAsync()));
}
else if (args.Length == 0)
{
    const string csvPath = "tickers.csv";
    if (!File.Exists(csvPath))
    {
        Console.Error.WriteLine($"No args provided and '{csvPath}' not found.");
        return 1;
    }

    var entries = CsvLoader.Load(csvPath);
    if (entries.Count == 0)
    {
        Console.Error.WriteLine("CSV file is empty or contains no valid entries.");
        return 1;
    }

    IPricingProvider pricingProvider = PricingProviderFactory.GetPricingProvider(config, useMock: true);
    IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(config, useMock: true);

    var list = new CyclicBucketList(
        bucketCount: entries.Count,
        config.CheckIntervalSeconds,
        config.MaxBuckets);

    foreach (var e in entries)
        list.AddTracker(new PriceTracker(e.Ticker, e.LowPrice, e.HighPrice, config.AlertTo, emailProvider, pricingProvider));

    await Task.WhenAll(list.Nodes().Select(n => n.Value.StartAsync()));
}
else
{
    Console.Error.WriteLine("Usage: pricing-alerts <ticker> <low_price> <high_price>");
    Console.Error.WriteLine("       pricing-alerts  (reads tickers.csv)");
    return 1;
}

return 0;
