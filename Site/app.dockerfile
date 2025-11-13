FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
COPY . /src
RUN dotnet publish -c release -o /app /src/Site

FROM node:20 AS node_builder
COPY . /src
WORKDIR /src/Site
RUN npm ci
RUN npx tailwindcss -i app.css -o wwwroot/app.css --minify

FROM nginx:alpine
COPY --from=builder /app/wwwroot/ /usr/share/nginx/html/
COPY --from=node_builder /src/Site/wwwroot/app.css /usr/share/nginx/html/app.css
COPY --from=builder /src/Site/docker/nginx_parts /etc/nginx/conf.d/parts
COPY --from=builder /src/Site/docker/site.conf /etc/nginx/conf.d/default.conf
VOLUME [ "/usr/share/nginx/html/config.json" ]