namespace PricingAlerts.PriceTracker;

public class Bucket
{
    private readonly List<PriceTracker> _trackers = new();
    private readonly int _intervalSeconds;

    public Bucket(int intervalSeconds) { _intervalSeconds = intervalSeconds; }

    public void Add(PriceTracker tracker) => _trackers.Add(tracker);

    public async Task StartAsync(CancellationToken ct = default)
    {
        await CheckAll();
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));
        while (await timer.WaitForNextTickAsync(ct))
            await CheckAll();
    }

    private Task CheckAll() => Task.WhenAll(_trackers.Select(t => t.CheckPrice()));
}
