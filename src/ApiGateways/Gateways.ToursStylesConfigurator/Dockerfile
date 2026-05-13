FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/Gateways.ToursStylesConfigurator/Gateways.ToursStyleConfigurator.csproj", "src/ApiGateways/Gateways.ToursStylesConfigurator/"]
RUN dotnet restore "src/ApiGateways/Gateways.ToursStylesConfigurator/Gateways.ToursStyleConfigurator.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Gateways.ToursStylesConfigurator"
RUN dotnet build "Gateways.ToursStyleConfigurator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateways.ToursStyleConfigurator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateways.ToursStyleConfigurator.dll"]
