.PHONY: up down config debug clean

CURRENT_DIR := $(dir $(lastword $(MAKEFILE_LIST)))
COMPOSE_FILES := docker-compose.yml
COMPOSE_ARGS := $(foreach file, $(COMPOSE_FILES), -f $(CURRENT_DIR)/$(file))

up:
	docker compose $(COMPOSE_ARGS) up --build -d

down:
	docker compose $(COMPOSE_ARGS) down

prod:
	set -a && . .prod.env && set +a && docker compose $(COMPOSE_ARGS) up --build -d

config:
	docker compose $(COMPOSE_ARGS) config

debug: docker-compose.yml
	docker compose -f docker-compose.debug.yml up --build -d
	cd vortex-web; ng serve

clean: docker-compose.yml
	docker compose -f docker-compose.debug.yml down
	docker compose down
