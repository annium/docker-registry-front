FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS builder
COPY . /src
RUN dotnet publish -c release -o /app /src/Server

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app
COPY --from=builder /app /app
VOLUME [ "/app/config.json", "/app/key.pem" ]
CMD ["/app/Server"]