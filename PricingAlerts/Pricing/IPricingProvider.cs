namespace PricingAlerts.Pricing;

public interface IPricingProvider
{
    Task<decimal> GetCurrentPrice(string ticker);
}
