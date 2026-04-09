# Pricing Alerts

Monitors B3 stock tickers via [Brapi](https://brapi.dev) and sends email alerts when prices cross configured thresholds. Supports single-ticker and batch modes.

## Running

```bash
# Single ticker
./pricing-alerts PETR4 22.59 22.67

# Batch mode — reads tickers.csv from the working directory
./pricing-alerts
```

CSV format (`ticker,low,high` header required):
```
ticker,low,high
PETR4,22.59,22.67
VALE3,60.00,65.00
```

## Configuration

`.env` file in the same directory as the executable:

```env
BRAPI_API_KEY=your_brapi_key

SMTP_HOST=your_smtp_host
SMTP_PORT=587
SMTP_USER=your_smtp_user
SMTP_PASSWORD=your_smtp_password
SMTP_SENDER=sender@email.com

ALERT_TO=destination@email.com
CHECK_INTERVAL_SECONDS=300
MAX_BUCKETS=10
LOG_LEVEL=Info
```

`LOG_LEVEL` accepts `Debug`, `Info`, or `Error`. Get a free Brapi API key at [brapi.dev](https://brapi.dev).

## Architecture

Provider-based design with constructor dependency injection. Each external concern is behind an interface:

- **`IPricingProvider`** — fetches stock prices (`BrapiPricingProvider`, `MockPricingProvider`)
- **`IEmailProvider`** — sends alerts (`SmtpEmailProvider`, `MockEmailProvider`)

Providers are instantiated via static factories and injected into `PriceTracker`. Swapping pricing sources or email backends requires only implementing the relevant interface.

## Technical Decisions

**Bucket-based scheduling with a cyclic doubly-linked list** — tickers are distributed round-robin across a fixed number of buckets. Each bucket owns an independent `PeriodicTimer` — using `PeriodicTimer` rather than `Task.Delay` ensures a slow check never causes ticks to queue up. All tickers within a bucket are checked concurrently via `Task.WhenAll`; since price fetching and email delivery are I/O-bound, checks overlap with no throughput penalty. Buckets can run at different polling intervals, which maps naturally to assets with different volatility profiles. The underlying cyclic doubly-linked list gives O(1) insertion and removal, and supports split and merge operations to rebalance bucket sizes as tickers are added or removed during execution.

**Docker-based build** — binaries are compiled inside a Docker container and extracted with `docker cp`. The only host dependencies are Docker and make; no .NET SDK required.

## Building from source

```bash
make build-linux        # Linux
make build-windows      # Windows
make build-macos-arm    # macOS Apple Silicon
make build-macos-intel  # macOS Intel
```
