using BuildingBlocks.Infrastructure.DataAccess.Cosmos;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

public record ToursStylesDbConfiguration : CosmosConfiguration
{
    public override string ConfigurationName => "TourStylesConfigurationDb";
}