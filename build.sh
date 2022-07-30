#!/bin/bash

# Load env variables from .env file
export $(grep -e '^VERSION=' .env | xargs)

# Bump version
NEW_VERSION="${VERSION%.*}.$((${VERSION##*.}+1))"

echo "Building version $NEW_VERSION. Current version is $VERSION"

sed -i -e "s/VERSION=$VERSION/VERSION=$NEW_VERSION/g" ./.env
export VERSION="$NEW_VERSION"

echo 'Building'

docker compose build
docker compose push

git tag -a "consensus-v$NEW_VERSION" -m "Build Docker images"
