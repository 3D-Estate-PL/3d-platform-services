FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Other steps in your Dockerfile

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/ImagesManagement/ImagesManagement.Api/ImagesManagement.Api.csproj", "src/Services/ImagesManagement/ImagesManagement.Api/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.HealthChecks.Dapr/BuildingBlocks.Infrastructure.DaprHealthChecks.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.HealthChecks.Dapr/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Observability.OpenTelemetry/BuildingBlocks.Infrastructure.Observability.OpenTelemetry.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Observability.OpenTelemetry/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.WebApi.HealthChecks/BuildingBlocks.Infrastructure.WebApi.HealthChecks.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.WebApi.HealthChecks/"]
COPY ["src/Services/ImagesManagement/ImagesManagement.Application/ImagesManagement.Application.csproj", "src/Services/ImagesManagement/ImagesManagement.Application/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.CQRS/BuildingBlocks.Abstractions.CQRS.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.CQRS/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Exceptions/BuildingBlocks.Abstractions.Exceptions.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Exceptions/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Application.EventBus/BuildingBlocks.Application.EventBus.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Application.EventBus/"]
COPY ["src/Services/ImagesManagement/ImagesManagement.Domain/ImagesManagement.Domain.csproj", "src/Services/ImagesManagement/ImagesManagement.Domain/"]
COPY ["src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/BuildingBlocks.Domain.DDD.csproj", "src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/"]
COPY ["src/Services/ImagesManagement/ImagesManagement.Infrastructure/ImagesManagement.Infrastructure.csproj", "src/Services/ImagesManagement/ImagesManagement.Infrastructure/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.AzureStorage/BuildingBlocks.Infrastructure.AzureStorage.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.AzureStorage/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Storage/BuildingBlocks.Abstractions.Storage.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Storage/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Cqrs.Mediatr/BuildingBlocks.Infrastructure.Cqrs.Mediatr.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Cqrs.Mediatr/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Logging/BuildingBlocks.Infrastructure.Logging.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Logging/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Validations/BuildingBlocks.Infrastructure.Validations.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Validations/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/BuildingBlocks.Infrastructure.DataAccess.Cosmos.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.EventBus.Dapr/BuildingBlocks.Infrastructure.EventBus.Dapr.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.EventBus.Dapr/"]
RUN dotnet restore "src/Services/ImagesManagement/ImagesManagement.Api/ImagesManagement.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/ImagesManagement/ImagesManagement.Api"
RUN dotnet build "ImagesManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ImagesManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImagesManagement.Api.dll"]
