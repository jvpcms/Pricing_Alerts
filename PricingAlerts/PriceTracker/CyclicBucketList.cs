using PricingAlerts.Logging;

namespace PricingAlerts.PriceTracker;

public class CyclicBucketList
{
    private readonly BucketNode _head;
    private BucketNode _cursor;
    public int BucketCount { get; }

    public CyclicBucketList(int bucketCount, int intervalSeconds, int maxBuckets)
    {
        BucketCount = Math.Clamp(bucketCount, 1, maxBuckets);

        _head = new BucketNode(new Bucket(index: 0, intervalSeconds: intervalSeconds));
        _head.Next = _head;
        _head.Prev = _head;

        for (int i = 1; i < BucketCount; i++)
        {
            var node = new BucketNode(new Bucket(index: i, intervalSeconds: intervalSeconds));
            var tail = _head.Prev;
            tail.Next = node;
            node.Prev = tail;
            node.Next = _head;
            _head.Prev = node;
        }

        _cursor = _head;
        Logger.Debug($"[CyclicBucketList] Created {BucketCount} bucket(s) (requested: {bucketCount}, max: {maxBuckets})");
    }

    public void AddTracker(PriceTracker tracker)
    {
        _cursor.Value.Add(tracker);
        _cursor = _cursor.Next;
    }

    public IEnumerable<BucketNode> Nodes()
    {
        var current = _head;
        do
        {
            yield return current;
            current = current.Next;
        } while (current != _head);
    }
}
