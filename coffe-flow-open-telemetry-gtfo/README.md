# Coffee Flow – Observability Playground

This project shows a practical observability setup for .NET microservices using the Grafana stack.
The goal is to learn how logs, metrics, and traces actually move from .NET services into Grafana using real tools and real configs.

```bash
# run via git bash/WSL - will create all the necessary s*ts.
bash docker-compose.sh 
```

```bash
# stop ALL
docker compose down

# build
docker compose up -d --build

# check
docker compose ps
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
## Service & Observability Endpoints

Host (browser / Postman):

Order Service       http://localhost:5002 
Inventory Service   http://localhost:5003 
Brew Service        http://localhost:5001 

Prometheus          http://localhost:9090
Alloy               http://localhost:12345
Loki                http://localhost:3100


Docker internal network (service to service): 

Order Service       http://orderservice:8080  
Inventory Service   http://inventoryservice:8080  
Brew Service        http://brewservice:8080 

OTLP (gRPC) Alloy   http://alloy:4317


# Test 
## Requests:
POST http://localhost:5002/order?coffeeType=latte
POST http://localhost:5002/orders

This request will generate a distributed trace flowing through:   
order-service -> inventory-service -> brew-service

## Observability Checks
### Logs (Loki)
Query logs for order-service:
GET http://localhost:3100/loki/api/v1/query?query={service="order-service"}

### Traces (Tempo)
Search for available traces:
GET http://localhost:3200/api/search

Fetch a specific trace using the traceId obtained from Loki:
GET http://localhost:3200/api/traces/{traceId} - traceId from Loki

### Metrics (Prometheus)
Open the Prometheus UI: http://localhost:9090
Available Metrics: 
order-service:
	coffee_orders_total

inventory-service:
	inventory_errors_total
	inventory_remaining_beans
	inventory_remaining_milk

brew-service:
	coffee_brew_duration_seconds
	coffee_brew_errors_total