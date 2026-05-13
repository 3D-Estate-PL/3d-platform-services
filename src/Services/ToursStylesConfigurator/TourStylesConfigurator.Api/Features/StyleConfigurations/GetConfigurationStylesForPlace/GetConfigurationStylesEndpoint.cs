using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStylesForPlace;

public class Endpoint : Endpoint<GetConfigurationStylesRequest, GetConfigurationStylesResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}");
        AllowAnonymous();
        Description(b => b
                .Produces<GetConfigurationStylesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.ExampleRequest = new GetConfigurationStylesRequest()
            {
               ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
            };
            s.Summary = "Get Configuration Styles";
            s.Description = "Get Configuration Styles";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetConfigurationStylesRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);

        var response = new GetConfigurationStylesResponse
        {
            ConfigurationId = configuration.Id,
        };
        
        
        if (configuration.GetPlace(request.Place).Styles != null)
        {
            foreach (var style in configuration.GetPlace(request.Place).Styles)
            {
                var styleConfigurationDto = style.Map();
                response.Styles.Add(styleConfigurationDto);
            } 
        }
       
        
        await SendAsync(response, cancellation: c);
    }
}