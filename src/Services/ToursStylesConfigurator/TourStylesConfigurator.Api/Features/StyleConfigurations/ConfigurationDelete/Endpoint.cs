using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationDelete;

public class Endpoint : Endpoint<DeleteConfigurationRequest, EmptyResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;

    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }


    public override void Configure()
    {
        Delete($"/{Paths.TourStyleConfigurations}");
        AllowAnonymous();
        Description(b => b
                .Accepts<DeleteConfigurationRequest>("application/json")
                .Produces<EmptyResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Delete configuration";
            s.Description = "Delete configuration";
            s.ExampleRequest = new DeleteConfigurationRequest
            {
                ConfigurationId = "id"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(DeleteConfigurationRequest request, CancellationToken c)
    {
        await _tourStyleConfigurationRepository.RemoveAsync<TourStyleConfiguration>(
            new TourStyleConfigurationIdentity(request.ConfigurationId));


        await SendAsync(new EmptyResponse(), cancellation: c);
    }
}