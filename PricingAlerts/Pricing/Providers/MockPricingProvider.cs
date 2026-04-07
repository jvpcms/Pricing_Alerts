namespace PricingAlerts.Pricing.Providers;

public class MockPricingProvider : IPricingProvider
{
    private static readonly Random _rng = new();

    public Task<decimal> GetCurrentPrice(string ticker)
    {
        var price = 100m + _rng.Next(-50, 51);
        Console.WriteLine($"[MockPricingProvider] GetCurrentPrice called for '{ticker}' -> {price}");
        return Task.FromResult(price);
    }
}
