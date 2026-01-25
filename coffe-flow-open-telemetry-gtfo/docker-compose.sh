#!/bin/bash
set -e

echo "Preparing folders for observability..." # просто лог в консоль

# папки под данные Loki и Prometheus
mkdir -p observability/loki/data
mkdir -p observability/prometheus/data

echo "Folders are ready. Starting full stack (services + observability)..." # ещё один лог

docker compose up -d # поднимает вообще всё

echo "Done. BrewService, OrderService, InventoryService, Alloy, Loki and Prometheus are running." # финал
