using PricingAlerts.Config;
using PricingAlerts.Pricing.Providers;

namespace PricingAlerts.Pricing;

public static class PricingProviderFactory
{
    public static IPricingProvider GetPricingProvider(AppConfig config, bool useMock = false)
    {
        if (useMock)
            return new MockPricingProvider();

        return new BrapiPricingProvider(config.BrapiApiKey);
    }
}
