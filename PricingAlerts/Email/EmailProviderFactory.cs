namespace PricingAlerts.Email;

public static class EmailProviderFactory
{
    public static IEmailProvider GetEmailProvider(bool useMock = true)
    {
        if (useMock)
            return new MockEmailProvider();

        throw new NotImplementedException("Real email provider not implemented yet.");
    }
}
