using BuildingBlocks.Domain.DDD;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

public class ConfigurationOwnerIdentity : DocumentIdentity<ConfigurationOwner>
{
    public ConfigurationOwnerIdentity(string id)
        : base(id)
    {
    }
}