FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/InteriorDesigners/InteriorDesigners.Api/InteriorDesigners.Api.csproj", "src/Services/InteriorDesigners/InteriorDesigners.Api/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Observability.OpenTelemetry/BuildingBlocks.Infrastructure.Observability.OpenTelemetry.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Observability.OpenTelemetry/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.WebApi.HealthChecks/BuildingBlocks.Infrastructure.WebApi.HealthChecks.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.WebApi.HealthChecks/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.HealthChecks.Dapr/BuildingBlocks.Infrastructure.DaprHealthChecks.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.HealthChecks.Dapr/"]
COPY ["src/Services/InteriorDesigners.Domain/InteriorDesigners.Domain.csproj", "src/Services/InteriorDesigners.Domain/"]
COPY ["src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/BuildingBlocks.Domain.DDD.csproj", "src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/"]
COPY ["src/Services/InteriorDesigners/InteriorDesigners.Application/InteriorDesigners.Application.csproj", "src/Services/InteriorDesigners/InteriorDesigners.Application/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Excel/BuildingBlocks.Abstractions.Excel.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Excel/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Application.EventBus/BuildingBlocks.Application.EventBus.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Application.EventBus/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.CQRS/BuildingBlocks.Abstractions.CQRS.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.CQRS/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Validations/BuildingBlocks.Infrastructure.Validations.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Validations/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Logging/BuildingBlocks.Infrastructure.Logging.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Logging/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.GoogleSheet/BuildingBlocks.Infrastructure.GoogleSheet.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.GoogleSheet/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.EventBus.Dapr/BuildingBlocks.Infrastructure.EventBus.Dapr.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.EventBus.Dapr/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/BuildingBlocks.Infrastructure.DataAccess.Cosmos.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Cqrs.Mediatr/BuildingBlocks.Infrastructure.Cqrs.Mediatr.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.Cqrs.Mediatr/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.AzureStorage/BuildingBlocks.Infrastructure.AzureStorage.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.AzureStorage/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Storage/BuildingBlocks.Abstractions.Storage.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Storage/"]
COPY ["src/Services/InteriorDesigners.Infrastructure/InteriorDesigners.Infrastructure.csproj", "src/Services/InteriorDesigners.Infrastructure/"]
RUN dotnet restore "src/Services/InteriorDesigners/InteriorDesigners.Api/InteriorDesigners.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/InteriorDesigners/InteriorDesigners.Api"
RUN dotnet build "InteriorDesigners.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InteriorDesigners.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InteriorDesigners.Api.dll"]
