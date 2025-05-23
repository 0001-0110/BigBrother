.PHONY: all prod clean

all:
	docker compose up --build -d

prod:
	docker compose -f docker-compose.prod.yml up --build -d

clean:
	docker compose -f docker-compose.prod.yml down
	docker compose down
