#!/bin/bash

# Load env variables from .env file
export $(grep -v '^#' .env | xargs)

export DOCKER_HOST=

# Bump version
NEW_VERSION="${VERSION%.*}.$((${VERSION##*.}+1))"

echo "Deploying version $NEW_VERSION. Current version is $VERSION"

sed -i -e "s/VERSION=$VERSION/VERSION=$NEW_VERSION/g" ./.env
export VERSION="$NEW_VERSION"

echo 'Building'

docker-compose build
docker-compose push
git tag -a "home-v$NEW_VERSION" -m "Deployment"

echo "Deploying to $DEPLOY_TO"

ssh root@$DEPLOY_TO 'docker container start socat'

export DOCKER_HOST="tcp://$DEPLOY_TO:9022"

docker-compose pull
docker-compose up --no-build -d

ssh root@$DEPLOY_TO 'docker container stop socat'

echo "Deployed $NEW_VERSION."
