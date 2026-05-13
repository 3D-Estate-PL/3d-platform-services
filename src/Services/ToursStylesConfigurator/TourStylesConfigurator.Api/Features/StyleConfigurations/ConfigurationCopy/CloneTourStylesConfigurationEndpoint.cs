using TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationCopy;

public class
    CloneTourStylesConfigurationEndpoint : Endpoint<CloneTourStylesConfigurationRequest,
        CloneTourStylesConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IEmailNotificationService _emailNotificationService;

    public CloneTourStylesConfigurationEndpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository,
        IEmailNotificationService emailNotificationService)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _emailNotificationService = emailNotificationService;
    }

    public override void Configure()
    {
        Post(
            $"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/clone");
        AllowAnonymous();
        Description(b => b
                .Accepts<CloneTourStylesConfigurationRequest>("application/json")
                .Produces<CloneTourStylesConfigurationResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Clone configuration";
            s.Description = "Clone configuration";
            s.ExampleRequest = new CloneTourStylesConfigurationRequest
            {
                ConfigurationId = "Id",
                InvestmentName = "Inwestycja A",
                ConfigurationOwner = new OwnerDto
                {
                    Email = "testowy@email.com",
                    Name = "Nazwa ownera"
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(CloneTourStylesConfigurationRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        var owner = await _tourStyleConfigurationRepository.FindOwnerByEmailAsync(request.ConfigurationOwner.Email);

        if (owner == null)
        {
            owner = ConfigurationOwner.New(request.ConfigurationOwner.Name, request.ConfigurationOwner.Email);
            await _tourStyleConfigurationRepository.UpsertOwnerAsync(owner);
        }
        
        var newConfiguration = configuration.Clone();
        newConfiguration.OwnerId = owner.Id;

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(newConfiguration);

        var response = new CloneTourStylesConfigurationResponse
        {
            StyleConfiguration = new StyleConfigurationDto
            {
                Id = newConfiguration.Id,
                Code = newConfiguration.Code,
                Status = newConfiguration.Status,
                InvestmentName = newConfiguration.InvestmentName,
                IsEditable = newConfiguration.IsEditable,
                Owner = new Owner
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    Name = owner.Name
                }
            },
        };
        
        if (configuration.GetPlace(PlaceType.Interior).Styles != null)
        {
            foreach (var style in configuration.GetPlace(PlaceType.Interior).Styles)
            {
                var styleConfigurationDto = style.Map();
                response.Styles.Add(styleConfigurationDto);
            } 
        }

        await _emailNotificationService.NewConfigurationEmailAsync(newConfiguration, owner, Env.EnvironmentName,c);

        await SendAsync(response, cancellation: c);
    }
}