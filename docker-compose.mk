CURRENT_DIR := $(dir $(lastword $(MAKEFILE_LIST)))
BIGBROTHER_COMPOSE_FILES := docker-compose.yml
BIGBROTHER_COMPOSE_ARGS := $(foreach file, $(BIGBROTHER_COMPOSE_FILES), -f $(CURRENT_DIR)/$(file))
