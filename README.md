# Pricing Alerts

Monitors B3 stock tickers and sends email alerts when prices cross high or low thresholds. Checks every 5 minutes by default.

## Running

Download the latest executable from [Releases](../../releases) and run:

**Single ticker:**
```bash
./pricing-alerts PETR4 22.59 22.67
#                ticker low   high
```

**Batch mode** — place a `tickers.csv` next to the executable and run with no args:
```bash
./pricing-alerts
```

CSV format (`ticker,low,high`, one per line, `#` for comments):
```
# ticker,low,high
PETR4,22.59,22.67
VALE3,60.00,65.00
```

Each ticker runs as a concurrent async task — all price fetches for the current interval fire in parallel, so batch mode has the same latency overhead as a single ticker.

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
