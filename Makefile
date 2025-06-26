.PHONY: up down config debug clean

include docker-compose.mk

up:
	docker compose $(BIGBROTHER_COMPOSE_ARGS) up --build -d

down:
	docker compose $(BIGBROTHER_COMPOSE_ARGS) down

prod:
	set -a && . .prod.env && set +a && docker compose $(BIGBROTHER_COMPOSE_ARGS) up --build -d

pull:
	docker compose $(BIGBROTHER_COMPOSE_ARGS) pull

build:
	docker compose $(BIGBROTHER_COMPOSE_ARGS) build

config:
	docker compose $(BIGBROTHER_COMPOSE_ARGS) config
