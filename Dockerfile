FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/OrderSystem.Api/OrderSystem.Api.csproj", "src/OrderSystem.Api/"]
COPY ["src/OrderSystem.Application/OrderSystem.Application.csproj", "src/OrderSystem.Application/"]
COPY ["src/OrderSystem.Domain/OrderSystem.Domain.csproj", "src/OrderSystem.Domain/"]
COPY ["src/OrderSystem.Infrastructure/OrderSystem.Infrastructure.csproj", "src/OrderSystem.Infrastructure/"]

RUN dotnet restore "src/OrderSystem.Api/OrderSystem.Api.csproj"

COPY . .
WORKDIR /src/src/OrderSystem.Api
RUN dotnet publish "OrderSystem.Api.csproj" \
    --configuration $BUILD_CONFIGURATION \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM base AS final
USER root
RUN apt-get update \
    && apt-get install --no-install-recommeds --yes curl \
    && rm -rf /var/lib/apt/lists/*

USER &APP_UID
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderSystem.Api.dll"]