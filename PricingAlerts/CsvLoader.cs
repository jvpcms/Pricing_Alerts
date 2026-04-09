using System.Globalization;

namespace PricingAlerts;

public record TickerEntry(string Ticker, decimal LowPrice, decimal HighPrice);

public static class CsvLoader
{
    public static List<TickerEntry> Load(string path)
    {
        var entries = new List<TickerEntry>();
        var lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split(',');
            if (parts.Length != 3
                || !decimal.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var low)
                || !decimal.TryParse(parts[2].Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var high))
            {
                Console.Error.WriteLine($"[CsvLoader] Skipping malformed line {i + 1}: '{line}'");
                continue;
            }

            entries.Add(new TickerEntry(parts[0].Trim(), low, high));
        }

        return entries;
    }
}
