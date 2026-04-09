namespace PricingAlerts.PriceTracker;

public class BucketNode
{
    public Bucket Value { get; }
    public BucketNode Next { get; internal set; } = null!;
    public BucketNode Prev { get; internal set; } = null!;

    public BucketNode(Bucket value) { Value = value; }
}
