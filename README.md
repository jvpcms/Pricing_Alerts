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
```

## Architecture

Provider-based design with constructor dependency injection. Each external concern is behind an interface:

- **`IPricingProvider`** — fetches stock prices (`BrapiPricingProvider`, `MockPricingProvider`)
- **`IEmailProvider`** — sends alerts (`SmtpEmailProvider`, `MockEmailProvider`)

Providers are instantiated via static factories and injected into `PriceTracker`. Swapping pricing sources or email backends requires only implementing the relevant interface.

`PriceTracker` owns the polling loop and a state machine (`Normal / Low / High`). Alerts fire only on state transitions — a price that stays below the low threshold does not spam the recipient on every check.

## Technical Decisions

**Polling with `PeriodicTimer`** — uses .NET's `PeriodicTimer` rather than a `Timer` callback or a `Task.Delay` loop. `PeriodicTimer` won't queue a new tick if the previous check is still running, preventing task accumulation under slow network conditions.

**Bucket-based scheduling with a cyclic doubly-linked list** — in batch mode, tickers are distributed round-robin across a fixed number of buckets. Each bucket owns an independent `PeriodicTimer` and checks all its tickers concurrently via `Task.WhenAll` — price fetching and email delivery are I/O-bound, so checks within a bucket overlap with no throughput penalty. Buckets can be configured with different polling intervals, which maps naturally to assets with different volatility profiles: high-frequency tickers can be isolated in faster buckets without affecting others. The underlying structure is a cyclic doubly-linked list, giving O(1) node insertion and removal. This also supports split and merge operations to rebalance bucket sizes as tickers are added or removed during program execution.

**Docker-based build** — binaries are compiled inside a Docker container and extracted with `docker cp`. The only host dependencies are Docker and make; no .NET SDK required.

## Building from source

```bash
make build-linux        # Linux
make build-windows      # Windows
make build-macos-arm    # macOS Apple Silicon
make build-macos-intel  # macOS Intel
```
