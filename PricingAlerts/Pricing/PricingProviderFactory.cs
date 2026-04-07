using PricingAlerts.Config;
using PricingAlerts.Pricing.Providers;

namespace PricingAlerts.Pricing;

public static class PricingProviderFactory
{
    public static IPricingProvider GetPricingProvider(bool useMock = false, AppConfig? config = null)
    {
        if (useMock)
            return new MockPricingProvider();

        if (config is null)
            throw new ArgumentNullException(nameof(config), "Config is required for real providers.");

        return new BrapiPricingProvider(config.BrapiApiKey);
    }
}
