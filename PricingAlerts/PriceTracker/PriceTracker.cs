using System.Globalization;
using PricingAlerts.Logging;
using PricingAlerts.Email;
using PricingAlerts.Pricing;

namespace PricingAlerts.PriceTracker;

public class PriceTracker
{
    public string Ticker { get; }
    private readonly string _ticker;
    private readonly decimal _lowPrice;
    private readonly decimal _highPrice;
    private readonly string _alertTo;
    private readonly IEmailProvider _emailProvider;
    private readonly IPricingProvider _pricingProvider;

    private PriceStatus _status = PriceStatus.Normal;

    public PriceTracker(
        string ticker,
        decimal lowPrice,
        decimal highPrice,
        string alertTo,
        IEmailProvider emailProvider,
        IPricingProvider pricingProvider)
    {
        Ticker = ticker;
        _ticker = ticker;
        _lowPrice = lowPrice;
        _highPrice = highPrice;
        _alertTo = alertTo;
        _emailProvider = emailProvider;
        _pricingProvider = pricingProvider;
    }

    internal async Task CheckPrice()
    {
        decimal price;
        try
        {
            price = await _pricingProvider.GetCurrentPrice(_ticker);
        }
        catch (Exception ex)
        {
            Logger.Error($"[PriceTracker] Failed to fetch price for '{_ticker}': {ex.Message}");
            return;
        }

        var priceStr = price.ToString(CultureInfo.InvariantCulture);
        var lowStr   = _lowPrice.ToString(CultureInfo.InvariantCulture);
        var highStr  = _highPrice.ToString(CultureInfo.InvariantCulture);

        Logger.Debug($"[PriceTracker] {_ticker} current price: R$ {priceStr}");

        var newStatus = price < _lowPrice ? PriceStatus.Low
                      : price > _highPrice ? PriceStatus.High
                      : PriceStatus.Normal;

        Logger.Debug($"[PriceTracker] {_ticker} status: {_status} -> {newStatus}");

        if (newStatus == _status)
            return;

        _status = newStatus;

        var (subject, content) = newStatus switch
        {
            PriceStatus.Low  => ($"{_ticker} Price Alert — Buy Signal",  $"{_ticker} is at R$ {priceStr}, below the low threshold of R$ {lowStr}. Consider buying."),
            PriceStatus.High => ($"{_ticker} Price Alert — Sell Signal", $"{_ticker} is at R$ {priceStr}, above the high threshold of R$ {highStr}. Consider selling."),
            _                => ($"{_ticker} Price Alert — Back to Normal", $"{_ticker} is back to R$ {priceStr}, within the normal range (R$ {lowStr} – R$ {highStr})."),
        };

        if (newStatus == PriceStatus.High)
            Logger.Info($"[PriceTracker] {_ticker} crossed above high threshold: R$ {priceStr} > R$ {highStr}");
        else if (newStatus == PriceStatus.Low)
            Logger.Info($"[PriceTracker] {_ticker} crossed below low threshold: R$ {priceStr} < R$ {lowStr}");

        try
        {
            await _emailProvider.SendEmail(_alertTo, subject, content);
        }
        catch (Exception ex)
        {
            Logger.Error($"[PriceTracker] Failed to send email: {ex.Message}");
        }
    }
}
