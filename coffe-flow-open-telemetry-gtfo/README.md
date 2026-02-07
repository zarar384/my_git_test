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
http://orderservice:8080  
http://inventoryservice:8080  
http://brewservice:8080  
http://alloy:4318


Test request:
POST http://localhost:5002/order?coffeeType=latte