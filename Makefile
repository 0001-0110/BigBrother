.PHONY: all clean

all:
	docker compose up --build -d

clean:
	docker compose down
