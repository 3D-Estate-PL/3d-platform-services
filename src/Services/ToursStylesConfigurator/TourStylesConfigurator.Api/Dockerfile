FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/ToursStylesConfigurator/TourStylesConfigurator.Api/TourStylesConfigurator.Api.csproj", "src/Services/ToursStylesConfigurator/TourStylesConfigurator.Api/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/BuildingBlocks.Infrastructure.DataAccess.Cosmos.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.DataAccess.Cosmos/"]
COPY ["src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/BuildingBlocks.Domain.DDD.csproj", "src/BuildingBlocks/Domain/BuildingBlocks.Domain.DDD/"]
COPY ["src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.GoogleSheet/BuildingBlocks.Infrastructure.GoogleSheet.csproj", "src/BuildingBlocks/Infrastructure/BuildingBlocks.Infrastructure.GoogleSheet/"]
COPY ["src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Excel/BuildingBlocks.Abstractions.Excel.csproj", "src/BuildingBlocks/Application/BuildingBlocks.Abstractions.Excel/"]
RUN dotnet restore "src/Services/ToursStylesConfigurator/TourStylesConfigurator.Api/TourStylesConfigurator.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/ToursStylesConfigurator/TourStylesConfigurator.Api"
RUN dotnet build "TourStylesConfigurator.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TourStylesConfigurator.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TourStylesConfigurator.Api.dll"]
