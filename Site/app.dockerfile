FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as builder
COPY . /src
RUN dotnet publish -c release -o /app /src

FROM node:16-alpine as node_builder
COPY . /src
RUN cd /src && npx tailwindcss -i app.css -o wwwroot/app.css --minify

FROM nginx:alpine
COPY --from=builder /app/wwwroot/ /usr/share/nginx/html/
COPY --from=node_builder /src/wwwroot/app.css /usr/share/nginx/html/app.css
COPY --from=builder /src/docker/nginx_parts /etc/nginx/conf.d/parts
COPY --from=builder /src/docker/site.conf /etc/nginx/conf.d/default.conf
VOLUME [ "/usr/share/nginx/html/config.json" ]