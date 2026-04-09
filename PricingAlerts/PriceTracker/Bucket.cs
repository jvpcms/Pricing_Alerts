using System.Runtime.CompilerServices;
using PricingAlerts.Logging;

namespace PricingAlerts.PriceTracker;

public class Bucket
{
    private readonly List<PriceTracker> _trackers = new();
    private readonly int _intervalSeconds;
    private readonly int _index;
    private string Id => $"#{_index}@{RuntimeHelpers.GetHashCode(this):x8}";

    public Bucket(int index, int intervalSeconds)
    {
        _index = index;
        _intervalSeconds = intervalSeconds;
        Logger.Debug($"[Bucket {Id}] Created | interval: {_intervalSeconds}s");
    }

    public void Add(PriceTracker tracker)
    {
        _trackers.Add(tracker);
        Logger.Debug($"[Bucket {Id}] Tracker added: {tracker.Ticker} ({_trackers.Count} total)");
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        Logger.Debug($"[Bucket {Id}] Starting | {_trackers.Count} trackers | interval: {_intervalSeconds}s");
        await CheckAll();
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(_intervalSeconds));
        while (await timer.WaitForNextTickAsync(ct))
            await CheckAll();
    }

    private Task CheckAll()
    {
        Logger.Debug($"[Bucket {Id} | {DateTime.Now:HH:mm:ss.fff}] Checking {_trackers.Count} trackers");
        return Task.WhenAll(_trackers.Select(t => t.CheckPrice()));
    }
}
