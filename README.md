# Pricing Alerts

Monitors a B3 stock ticker and sends an email alert when the price crosses a high or low threshold. Checks every 5 minutes by default.

## Running

Download the latest executable from [Releases](../../releases) and run:

```bash
./pricing-alerts PETR4 22.59 22.67
#                ticker low   high

```
## Dependencies

- **Docker** — required to build the executable
- **make** — to run build commands
- An **SMTP server** to send emails

## Configuration

Create a `.env` file in the same directory as the executable:

```env
BRAPI_API_KEY=your_brapi_key

SMTP_HOST=your_smtp_host
SMTP_PORT=587
SMTP_USER=your_smtp_user
SMTP_PASSWORD=your_smtp_password
SMTP_SENDER=sender@email.com

ALERT_TO=destination@email.com
CHECK_INTERVAL_SECONDS=300
```

Get a free Brapi API key at [brapi.dev](https://brapi.dev). Any SMTP server works (Gmail, Outlook, SendGrid, etc.).

## Architecture

Provider-based design with constructor dependency injection. Each concern is behind an interface with swappable implementations:

- **`IPricingProvider`** — fetches stock prices (Brapi, Mock)
- **`IEmailProvider`** — sends alerts (SMTP, Mock)

Providers are wired up via static factories and injected into `PriceTracker`, which owns the polling loop and state machine. Adding a new pricing source or email backend only requires implementing the relevant interface and registering it in its factory.

## Building from source

Requires Docker and make.

```bash
make build-linux        # Linux
make build-windows      # Windows
make build-macos-arm    # macOS Apple Silicon
make build-macos-intel  # macOS Intel
```
