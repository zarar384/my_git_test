#!/bin/bash
set -e

echo "Preparing folders for observability..." # просто лог в консоль

# папки под данные Loki, Prometheus и Tempo
mkdir -p observability/loki/data
mkdir -p observability/prometheus/data
mkdir -p observability/tempo/data 

echo "Folders are ready. Starting full stack (services + observability)..." # ещё один лог

docker compose up -d --build # поднимает вообще всё

# Полная пересборка БЕЗ использования cache
# docker compose build --no-cache
# docker compose up -d

echo "Done. BrewService, OrderService, InventoryService, Alloy, Loki, Prometheus and Tempo are running." # финал
