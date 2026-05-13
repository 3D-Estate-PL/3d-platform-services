using TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationEdit;

public class EditTourStylesConfigurationEndpoint : Endpoint<EditTourStylesConfigurationRequest, EditTourStylesConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public EditTourStylesConfigurationEndpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Put(
            $"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<EditTourStylesConfigurationRequest>("application/json")
                .Produces<EditTourStylesConfigurationResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Edit base configuration data";
            s.Description =  "Edit base configuration data";
            s.ExampleRequest = new EditTourStylesConfigurationRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                Owner = new OwnerDto
                {
                    Email = "a123@wp.pl",
                    Name = "Nazwa developer1",
                },
                InvestmentName = "Nazwa inwestycji"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(EditTourStylesConfigurationRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        var owner = await _tourStyleConfigurationRepository.GetOwnerAsync(new ConfigurationOwnerIdentity(configuration.OwnerId));
        owner.Email = request.Owner.Email;
        owner.Name = request.Owner.Name;
        configuration.InvestmentName = request.InvestmentName;
        
        await _tourStyleConfigurationRepository.UpsertOwnerAsync(owner);
        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendAsync(new EditTourStylesConfigurationResponse()
        {
            ConfigurationId = configuration.Id,
            Owner = new OwnerDto
            {
                Name = owner.Name,
                Email = owner.Email
            },
            InvestmentName = configuration.InvestmentName
        }, cancellation: c);
    }
}