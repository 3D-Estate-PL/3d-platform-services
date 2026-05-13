using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleDelete;

public class Endpoint : Endpoint<DeleteConfigurationStyleRequest, EmptyResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Delete($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<DeleteConfigurationStyleRequest>("application/json")
                .Produces<EmptyResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Delete style configuration";
            s.Description = "Delete style configuration";
            s.ExampleRequest = new DeleteConfigurationStyleRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                Place = PlaceType.Interior,
                ConfigurationStyleId = "InsertId"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(DeleteConfigurationStyleRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var styleConfigurationItem = configuration.GetPlace(request.Place).
            Styles.Single(x => x.Id == request.ConfigurationStyleId);
        
        configuration.GetPlace(request.Place).RemoveStyle(styleConfigurationItem);
        
        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new EmptyResponse(), cancellation: c);
    }
}