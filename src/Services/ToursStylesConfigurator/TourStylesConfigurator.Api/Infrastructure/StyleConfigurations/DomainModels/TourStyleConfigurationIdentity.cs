using BuildingBlocks.Domain.DDD;

namespace TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

public class TourStyleConfigurationIdentity : DocumentIdentity<TourStyleConfiguration>
{
    public TourStyleConfigurationIdentity(string id)
        : base(id)
    {
    }
}