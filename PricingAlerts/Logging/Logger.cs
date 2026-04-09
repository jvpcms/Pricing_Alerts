namespace PricingAlerts.Logging;

public enum LogLevel { Debug = 0, Info = 1, Error = 2 }

public static class Logger
{
    private static LogLevel _level = LogLevel.Info;

    public static void Configure(LogLevel level) => _level = level;

    public static void Debug(string message)
    {
        if (_level <= LogLevel.Debug)
            Console.WriteLine($"[DEBUG] {message}");
    }

    public static void Info(string message)
    {
        if (_level <= LogLevel.Info)
            Console.WriteLine($"[INFO] {message}");
    }

    public static void Error(string message) =>
        Console.Error.WriteLine($"[ERROR] {message}");
}
