using PricingAlerts.Email;
using PricingAlerts.Pricing;

namespace PricingAlerts.PriceTracker;

public class PriceTracker
{
    private readonly string _ticker;
    private readonly decimal _lowPrice;
    private readonly decimal _highPrice;
    private readonly string _alertTo;
    private readonly IEmailProvider _emailProvider;
    private readonly IPricingProvider _pricingProvider;

    private readonly int _intervalSeconds;
    private PriceStatus _status = PriceStatus.Normal;

    public PriceTracker(
        string ticker,
        decimal lowPrice,
        decimal highPrice,
        string alertTo,
        IEmailProvider emailProvider,
        IPricingProvider pricingProvider,
        int intervalSeconds = 300)
    {
        _ticker = ticker;
        _lowPrice = lowPrice;
        _highPrice = highPrice;
        _alertTo = alertTo;
        _emailProvider = emailProvider;
        _pricingProvider = pricingProvider;
        _intervalSeconds = intervalSeconds;
    }

    public async Task StartAsync()
    {
        await CheckPrice();

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));
        while (await timer.WaitForNextTickAsync())
            await CheckPrice();
    }

    private async Task CheckPrice()
    {
        decimal price;
        try
        {
            price = await _pricingProvider.GetCurrentPrice(_ticker);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[PriceTracker] Failed to fetch price for '{_ticker}': {ex.Message}");
            return;
        }

        Console.WriteLine($"[PriceTracker] {_ticker} current price: R$ {price}");

        var newStatus = price < _lowPrice ? PriceStatus.Low
                      : price > _highPrice ? PriceStatus.High
                      : PriceStatus.Normal;

        Console.WriteLine($"[PriceTracker] Status: {_status} -> {newStatus}");

        if (newStatus == _status)
            return;

        _status = newStatus;

        var (subject, content) = newStatus switch
        {
            PriceStatus.Low  => ($"{_ticker} Price Alert — Buy Signal",  $"{_ticker} is at R$ {price}, below the low threshold of R$ {_lowPrice}. Consider buying."),
            PriceStatus.High => ($"{_ticker} Price Alert — Sell Signal", $"{_ticker} is at R$ {price}, above the high threshold of R$ {_highPrice}. Consider selling."),
            _                => ($"{_ticker} Price Alert — Back to Normal", $"{_ticker} is back to R$ {price}, within the normal range (R$ {_lowPrice} – R$ {_highPrice})."),
        };

        try
        {
            await _emailProvider.SendEmail(_alertTo, subject, content);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[PriceTracker] Failed to send email: {ex.Message}");
        }
    }
}
