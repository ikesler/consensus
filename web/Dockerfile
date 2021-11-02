FROM node:17 AS build-front
WORKDIR /src
COPY package.json package-lock.json ./
RUN npm i
COPY . .
ENV NODE_OPTIONS=--openssl-legacy-provider
RUN npm run build

FROM nginx
WORKDIR /public
COPY --from=build-front /src/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build-front /src/dist .
COPY --from=build-front /src/docker-entrypoint.sh /docker-entrypoint.sh

ENTRYPOINT ["sh", "/docker-entrypoint.sh"]
