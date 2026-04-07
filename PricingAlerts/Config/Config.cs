namespace PricingAlerts.Config;

public class Config
{
    private readonly string _envFile;
    public Config(string envFile = ".env")
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
