using PricingAlerts.Email;

IEmailProvider emailProvider = EmailProviderFactory.GetEmailProvider(useMock: true);
emailProvider.SendEmail("user@example.com", "Price Alert", "The price of AAPL dropped below $150.");
