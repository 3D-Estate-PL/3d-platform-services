using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

public class ToursStylesConfiguratorDbContext : CosmosContext
{
    public ToursStylesConfiguratorDbContext(ToursStylesDbConfiguration cosmosDbClient) 
        : base(cosmosDbClient)
    {
        Map<ConfigurationOwner>("toursStyleConfigurations-owners");
        Map<TourStyleConfiguration>("toursStyleConfigurations-configurations");
    }
}