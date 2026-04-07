namespace PricingAlerts.Config;

public class AppConfig
{
    private readonly string _envFile;

    public AppConfig(string envFile = ".env")
    {
        _envFile = envFile;
        EnvLoader.Load(_envFile);
        ValidateRequired();
    }

    public string BrapiApiKey => GetRequired("BRAPI_API_KEY");
    public string SmtpHost => GetRequired("SMTP_HOST");
    public int SmtpPort => int.Parse(GetRequired("SMTP_PORT"));
    public string SmtpUser => GetRequired("SMTP_USER");
    public string SmtpPassword => GetRequired("SMTP_PASSWORD");
    public string SmtpSender => GetRequired("SMTP_SENDER");

    private static string GetRequired(string key) =>
        Environment.GetEnvironmentVariable(key)!;

    private static void ValidateRequired()
    {
        string[] required = ["BRAPI_API_KEY", "SMTP_HOST", "SMTP_PORT", "SMTP_USER", "SMTP_PASSWORD", "SMTP_SENDER"];

        var missing = required
            .Where(k => string.IsNullOrEmpty(Environment.GetEnvironmentVariable(k)))
            .ToList();

        if (missing.Count > 0)
            throw new InvalidOperationException(
                $"Missing required environment variables: {string.Join(", ", missing)}");
    }
}
