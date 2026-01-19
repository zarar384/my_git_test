open https://github.com/grafana/loki/releases
download loki-windows-amd64.zip AND promtail-windows-amd64.zip

create folders:
\loki\data
	\data\rules
	\data\chunks

create config files
loki-config.yaml
promtail-config.yaml


RUN loki
cd D:\observability\loki
.\loki-windows-amd64.exe --% -config.file=loki-config.yaml

OPEN AND WAIT 15 SEC
http://localhost:3100/ready

RUN promtail
.\promtail-windows-amd64.exe --% -config.file=promtail-config.yaml
http://localhost:9080/targets
