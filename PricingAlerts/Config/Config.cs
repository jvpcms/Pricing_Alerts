namespace PricingAlerts.Config;

public class AppConfig
{
    private readonly string _envFile;
    public AppConfig(string envFile = ".env")
    {
        _envFile = envFile;
    }

    public string BrapiApiKey
    {
        get
        {
            EnvLoader.Load(_envFile);
            return Environment.GetEnvironmentVariable("BRAPI_API_KEY")!;
        }
    }
}
