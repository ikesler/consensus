#!/bin/bash

# Load env variables from .env file
export $( grep -e '^VERSION=' .env | xargs)
export $( grep -e '^DOCKER_CONTEXT=' .env | xargs)

echo "Deploying $VERSION to $DOCKER_CONTEXT context"

docker compose up -d --no-build

echo "Deployed $VERSION."
