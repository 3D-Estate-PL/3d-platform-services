using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;

public class Endpoint : Endpoint<CreateNewConfigurationRequest, CreateNewStyleConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly ILogger<Endpoint> _logger;

    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IEmailNotificationService emailNotificationService, ILogger<Endpoint> logger)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _emailNotificationService = emailNotificationService;
        _logger = logger;
    }
    
    public override void Configure()
    {
        Post($"/{Paths.TourStyleConfigurations}");
        AllowAnonymous();
        Description(b => b
                .Accepts<CreateNewConfigurationRequest>("application/json")
                .Produces<CreateNewStyleConfigurationResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Create New Style Configuration";
            s.Description = "Create New Style Configuration For Developer";
            s.ExampleRequest = new CreateNewConfigurationRequest()
            {
                InvestmentName = "Sample Investment",
                ConfigurationOwner = new OwnerDto
                {
                    Name = "Sample Developer",
                    Email = "developer@3destate.pl"
                }
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(CreateNewConfigurationRequest request, CancellationToken c)
    {
        var owner = await _tourStyleConfigurationRepository.FindOwnerByEmailAsync(request.ConfigurationOwner.Email);

        if (owner == null)
        {
            owner = ConfigurationOwner.New(request.ConfigurationOwner.Name, request.ConfigurationOwner.Email);
            await _tourStyleConfigurationRepository.UpsertOwnerAsync(owner);
        }

        var interiorDesignerDomain = HttpContext.GetContext();

        var styleConfiguration = TourStyleConfiguration.New(owner.Id, request.InvestmentName,interiorDesignerDomain);

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(styleConfiguration);

        await _emailNotificationService.NewConfigurationEmailAsync(styleConfiguration,owner,Env.EnvironmentName,c);
        
        await SendAsync(new CreateNewStyleConfigurationResponse
        {
            StyleConfiguration = new StyleConfigurationDto
            {
                Id = styleConfiguration.Id,
                Code = styleConfiguration.Code,
                Status = styleConfiguration.Status,
                InvestmentName = styleConfiguration.InvestmentName,
                IsEditable = styleConfiguration.IsEditable,
                Owner = new Owner
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    Name = owner.Name
                }
            }
        }, cancellation: c);
    }
}