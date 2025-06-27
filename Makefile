.PHONY: up down prod

include docker-compose.mk

up: down
	docker compose up --build -d

down:
	docker compose down

prod: down
	docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build -d
