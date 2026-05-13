using Azure.Storage.Blobs;
using TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationAdd;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.ConfigurationsRecovery;

public class Endpoint : Endpoint<RecoveryConfigurationsRequest, GetTourStylesConfigurationsResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private IEmailNotificationService _emailNotificationService;

    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IEmailNotificationService emailNotificationService)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _emailNotificationService = emailNotificationService;
    }

    public override void Configure()
    {
        Post($"/{Paths.TourStyleConfigurations}/recovery");
        AllowAnonymous();
        Description(b => b
                .Accepts<RecoveryConfigurationsRequest>("application/json")
                .Produces<GetTourStylesConfigurationsResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Get Configurations Tours Styles ";
            s.Description =  "Get Configurations Tours Styles";
            s.ExampleRequest = new RecoveryConfigurationsRequest()
            {
                Email = "developer@3destate.pl"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(RecoveryConfigurationsRequest request, CancellationToken c)
    {
        var configurationOwner = await _tourStyleConfigurationRepository.GetOwnerByEmailAsync(request.Email);
        var configurations = await _tourStyleConfigurationRepository.FindConfigurationsByOwnerAsync(configurationOwner.Id);


        var items = new List<string>();

        var loopItemTemplateUrl = "https://3dnotificationstaging.blob.core.windows.net/email-templates/email-template-recovery-codes-loop-item.html";
        var interiorDesigner = configurations.FirstOrDefault()?.InteriorDesignerCode;
        var blobClient = new BlobClient(new Uri(loopItemTemplateUrl));
        var downloadResult = await blobClient.DownloadContentAsync(c);
        var itemTemplate = downloadResult.Value.Content.ToString();
        
        foreach (var configuration in configurations)
        {
           items.Add(await CreateContentForConfiguration(itemTemplate, configuration));
        }

        await SendEmail(c, configurationOwner, string.Join(' ', items),interiorDesigner);
        await SendAsync(new GetTourStylesConfigurationsResponse()
        {
           
        }, cancellation: c);
    }

    private async Task<string> CreateContentForConfiguration(string itemTemplate, TourStyleConfiguration configuration)
    {
        itemTemplate= itemTemplate.Replace("{{CODE}}", configuration.Code);
        itemTemplate = itemTemplate.Replace("{{DATA}}", configuration.CreatedDate.HasValue ? configuration.CreatedDate.Value.ToString("dd-MM-yyyy") : "");
        return itemTemplate;
    }
    
    
    
    private async Task SendEmail(CancellationToken c, ConfigurationOwner owner, string loopItem, string interiorDesigner)
    {
        var loopItemTemplateUrl = "https://3dnotificationstaging.blob.core.windows.net/email-templates/email-template-recovery-codes-loop-item.html";
        try
        {
            var sendEmailNotificationCommand = new SendEmailNotificationCommand
            {
                ContentTemplate = new ContentTemplate
                {
                    TemplateUrl =
                        "https://3dnotificationstaging.blob.core.windows.net/email-templates/email-template-recovery-codes.html",
                    Parameters = new Dictionary<string, string>
                    {
                        {
                            "LOOP_ITEM", loopItem
                        },
                        {
                            "EMAIL", owner.Email
                        },
                        {"CONFIGURATION_LINK",$"https://{InteriorDesignerContextProvider.GetHostName(interiorDesigner,Env.EnvironmentName)}.3destate.pl"}
                    }
                },
                Recipients = owner.Email,
                SourceServiceCode = "TourConfiguration",
                NotificationId = Guid.NewGuid(),
                SenderEmail = "system@3destate.pl",
                SenderName = "3D Estate Configurator",
                Title = $"Przesyłamy wszystkie twoje konfiguracje stylów 3D Estate. Nie odpowiadaj na tą wiadomość.",
            };
            await _emailNotificationService.SendEmailAsync(sendEmailNotificationCommand, c);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}