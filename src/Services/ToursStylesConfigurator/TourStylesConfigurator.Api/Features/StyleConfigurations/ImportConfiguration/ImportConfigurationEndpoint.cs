using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ImportConfiguration;

public class Endpoint : Endpoint<ImportStylesConfigurationRequest, ImportStylesConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Post($"/{Paths.TourStyleConfigurations}/import");
        AllowAnonymous();
        Description(b => b
                .Accepts<ImportStylesConfigurationRequest>("application/json")
                .Produces<ImportStylesConfigurationResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Import Configuration Styles";
            s.Description =  "Import Configuration Styles";
            s.ExampleRequest = new ImportStylesConfigurationRequest()
            {
                    Code = "000000",
                    Email = "developer@3destate.pl"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(ImportStylesConfigurationRequest request, CancellationToken c)
    {
        var configurationOwner = await _tourStyleConfigurationRepository.GetOwnerByEmailAsync(request.Email);
        var configuration = await _tourStyleConfigurationRepository.FindConfigurationByCodeAsync(configurationOwner.Id, request.Code);

        await SendAsync(new ImportStylesConfigurationResponse()
        {
            StyleConfiguration = new CreateStyleConfigurationDto
            {
                Id = configuration.Id,
                Code = configuration.Code,
                Status = configuration.Status,
                InvestmentName = configuration.InvestmentName,
                IsEditable = configuration.IsEditable,
                Owner = new Owner
                {
                    Id = configurationOwner.Id,
                    Email = configurationOwner.Email
                }
            }
        }, cancellation: c);
    }
}