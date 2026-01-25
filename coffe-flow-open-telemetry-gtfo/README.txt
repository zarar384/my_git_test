# Coffee Flow – Observability Playground

This project shows a practical observability setup for .NET microservices using the Grafana stack.
The goal is to learn how logs, metrics, and traces actually move from .NET services into Grafana using real tools and real configs.

```bash
# run via git bash/WSL
bash docker-compose.sh - will create all the necessary s*ts.
```

```bash
# stop ALL
docker compose down
```

## Business Flow
```
Client
  |
  v
order-service
  |
  v
inventory-service
  |
  v
brew-service
```

##  Observability Flow

```
order-service     \
inventory-service  ---> OTLP ---> Alloy ---> Tempo (traces)
brew-service      /              |
                                  ---> Prometheus (metrics)

order-service     \
inventory-service  -----> Serilog HTTP ---> Loki (HTTP push API)
brew-service      /
```

## ABOUT Loki (logs):
```
.NET service
   |
   +--> stdout (console)
   |
   +--> HTTP ---> Loki (http://loki:3100)
```
