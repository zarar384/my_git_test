#!/bin/bash

set -e

echo "Preparing folders for Observability stack..."

# корневая папка проекта (где лежит этот скрипт)
BASE_DIR="$(pwd)"

# ALLOY
mkdir -p "$BASE_DIR/alloy" # папка под конфиг Alloy

# LOKI
mkdir -p "$BASE_DIR/loki/data/chunks" # чанки логов Loki
mkdir -p "$BASE_DIR/loki/data/rules"  # правила Loki (алерты, если понадобятся)
mkdir -p "$BASE_DIR/loki/logs"         # тестовые и реальные логи

# TEMPO
mkdir -p "$BASE_DIR/tempo/data/traces" # трейсы Tempo
mkdir -p "$BASE_DIR/tempo/data/wal"    # WAL для Tempo

# MIMIR
mkdir -p "$BASE_DIR/mimir/data"    # данные метрик Mimir
mkdir -p "$BASE_DIR/mimir/alerts" # алерт-правила (rules.yaml)

# GRAFANA
mkdir -p "$BASE_DIR/grafana/provisioning/datasources" # автоподключение источников данных
mkdir -p "$BASE_DIR/grafana/provisioning/dashboards"  # автоподключение дашбордов
mkdir -p "$BASE_DIR/grafana/dashboards"                # сами json-дашборды

echo "Folders created."

echo "Downloading configsx..." # if not exists

# TEMPO CONFIG
if [ ! -f "$BASE_DIR/tempo/tempo.yaml" ]; then
  curl -sSL -o "$BASE_DIR/tempo/tempo.yaml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/tempo/tempo.yml
fi

# ALLOY CONFIG
if [ ! -f "$BASE_DIR/alloy/config.alloy" ]; then
  curl -sSL -o "$BASE_DIR/alloy/config.alloy" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/alloy/config.alloy
fi

# LOKI CONFIG
if [ ! -f "$BASE_DIR/loki/loki-config.yaml" ]; then
  curl -sSL -o "$BASE_DIR/loki/loki-config.yaml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/loki/config.yml
fi

# PROMTAIL CONFIG (если вдруг захочешь вместо Alloy)
if [ ! -f "$BASE_DIR/loki/promtail-config.yaml" ]; then
  curl -sSL -o "$BASE_DIR/loki/promtail-config.yaml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/loki/config.yml
fi

# MIMIR CONFIG
if [ ! -f "$BASE_DIR/mimir/mimir.yaml" ]; then
  curl -sSL -o "$BASE_DIR/mimir/mimir.yaml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/mimir/config.yaml
fi

# GRAFANA DASHBOARD + PROVISIONING
if [ ! -f "$BASE_DIR/grafana/dashboards/ShoeHub_Dashboard.json" ]; then
  curl -sSL -o "$BASE_DIR/grafana/dashboards/ShoeHub_Dashboard.json" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/grafana/ShoeHub_Dashboard.json
fi

if [ ! -f "$BASE_DIR/grafana/provisioning/dashboards/dashboards.yml" ]; then
  curl -sSL -o "$BASE_DIR/grafana/provisioning/dashboards/dashboards.yml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/grafana/dashboards.yml
fi

if [ ! -f "$BASE_DIR/grafana/provisioning/datasources/datasources.yml" ]; then
  curl -sSL -o "$BASE_DIR/grafana/provisioning/datasources/datasources.yml" \
    https://raw.githubusercontent.com/aussiearef/grafana-udemy/main/grafana/datasources.yml
fi

echo "Configs ready."

echo "Setting permissions..." # чтобы докер не ныл

chmod -R 755 "$BASE_DIR"
chmod -R 777 "$BASE_DIR/tempo/data"
chmod -R 777 "$BASE_DIR/mimir/data"
chmod -R 777 "$BASE_DIR/loki/data"
chmod -R 777 "$BASE_DIR/loki/logs"

echo "Permissions set."

echo "Now running docker compose..."

# по умолчанию новый синтаксис Docker
DOCKER_CMD="docker compose"

# если передали --legacy, используем старый docker-compose
if [[ "$1" == "--legacy" ]]; then
  DOCKER_CMD="docker-compose"
fi

# запуск docker-compose из папки mimir (у тебя там уже есть yaml)
$DOCKER_CMD -f "$BASE_DIR/mimir/docker-compose.yaml" up -d

echo "Observability stack is starting"