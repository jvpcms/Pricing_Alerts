using System.Text.Json;

namespace PricingAlerts.Pricing.Providers;

public class BrapiPricingProvider : IPricingProvider
{
    private static readonly HttpClient _http = new();
    private readonly string _apiKey;

    public BrapiPricingProvider(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<decimal> GetCurrentPrice(string ticker)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://brapi.dev/api/quote/{ticker}");

        request.Headers.Add("Authorization", $"Bearer {_apiKey}");

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);

        var price = doc.RootElement
            .GetProperty("results")[0]
            .GetProperty("regularMarketPrice")
            .GetDecimal();

        return price;
    }
}
