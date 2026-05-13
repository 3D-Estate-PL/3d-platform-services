FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/Gateways.PlatformApiGateway/Gateways.PlatformApiGateway.csproj", "src/ApiGateways/Gateways.PlatformApiGateway/"]
RUN dotnet restore "src/ApiGateways/Gateways.PlatformApiGateway/Gateways.PlatformApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Gateways.PlatformApiGateway"
RUN dotnet build "Gateways.PlatformApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateways.PlatformApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateways.PlatformApiGateway.dll"]
