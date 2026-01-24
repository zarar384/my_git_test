[Alloy] ──scrape──> сервисы
   │
   ├──logs────────> Loki
   ├──traces──────> Tempo
   └──metrics─────> Mimir

Alloy = курьер - курьер возит посылки на склады
Loki / Tempo / Mimir = склады 
Grafana = витрина - показывает, что лежит на складах

скачать https://github.com/grafana/mimir/releases

проверить, если живой 
.\mimirtool-windows-amd64.exe --help

создай папки в /mimir
data, tsdb, rules, alerts

создай конфиг в /mimir
mimir.yaml
rules.yaml - для алерт-правил

создать докер компостер
docker-compose.yml

запуск 
docker compose up -d

проверка docker ps
http://localhost:9009/ready

проверка правил 
.\mimirtool-windows-amd64.exe rules lint rules.yaml

загрузка правил в мимир
.\mimirtool-windows-amd64.exe rules load rules.yaml `
  --address=http://localhost:9009 `
  --id=local

проверка, если правила реально подгрузились
.\mimirtool-windows-amd64.exe rules list --address=http://localhost:9009

или в браузере http://localhost:9009/prometheus/api/v1/rules


В туторе от графана делают так:
[Prometheus] ──scrape──> сервисы
      │
      └──remote_write──> [Mimir]

для этого нужен prometheus.yml, но я выбрал путь через Alloy

