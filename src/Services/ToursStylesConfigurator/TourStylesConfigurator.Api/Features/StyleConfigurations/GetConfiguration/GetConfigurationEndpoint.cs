using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfiguration;

public class Endpoint : Endpoint<GetConfigurationRequest, GetConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{code}}/styles");
        AllowAnonymous();
        Description(b => b
                .Produces<GetConfigurationResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.ExampleRequest = new GetConfigurationRequest
            {
                Code = "Q5XB2"
            };
            s.Summary = "Get Configuration Styles";
            s.Description = "Get Configuration Styles";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetConfigurationRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationByCodeAsync(request.Code);

      
        
        var response = new GetConfigurationResponse();

        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            foreach (var style in configuration.InteriorStyleConfiguration.Styles)
                response.Styles.Add(new TourConfigurationStyle
                {
                    Code = style.Code,
                    Name = style.CustomName
                });
        }

        await SendAsync(response, cancellation: c);
    }
}