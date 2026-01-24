запуск через git bash / WSL
bash docker-compose.sh - создаст всю необходимую мишуру

Grafana  http://localhost:3000
Loki     http://localhost:3100/ready
Tempo    http://localhost:3200/ready
Mimir    http://localhost:9009/ready

потом открываешь папку mimir и читаешь README про rules 
и для проверки в повершеле echo "hello from .NET app" >> logs/app.log
в графане в локи вбей {job="windows"}
если не работает, то ищи ошибку. С Богом!

observability-setup/
│
├── docker-compose.sh       # скрипт: создаёт папки и запускает всё
├── docker-compose.yaml     # поднимает Grafana + Loki + Tempo + Mimir + Alloy
│
├── alloy/
│   └── config.alloy        # агент: шлёт логи, трейсы и метрики
│
├── loki/
│   ├── config.yaml         # конфиг Loki
│   └── data/               # данные логов
│       └── chunks/
│
├── tempo/
│   ├── config.yaml         # конфиг Tempo
│   └── data/               # данные трейсов
│       ├── traces/
│       └── wal/
│
├── mimir/
│   ├── config.yaml         # конфиг Mimir
│   ├── rules.yaml          # алерт-правила
│   └── data/               # данные метрик
│
├── grafana/
│   ├── datasources.yaml    # авто-подключение Loki / Tempo / Mimir
│   └── dashboards.json     # один тестовый дашборд
│
└── logs/
    └── app.log             # логи .NET приложения (куда пишет Serilog)
