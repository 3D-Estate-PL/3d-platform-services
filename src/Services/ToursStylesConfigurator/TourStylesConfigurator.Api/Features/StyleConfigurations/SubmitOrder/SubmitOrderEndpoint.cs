using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Owners;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.SubmitOrder;

public class SubmitOrderEndpoint : Endpoint<SubmitOrderRequest, EmptyResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;
    public SubmitOrderEndpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, 
        IEmailNotificationService emailNotificationService, 
        IStyleDefaultProductsProvider styleDefaultProductsProvider, IGetRoomTypesQuery getRoomTypesQuery)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _emailNotificationService = emailNotificationService;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _getRoomTypesQuery = getRoomTypesQuery;
    }

    public override void Configure()
    {
        Post(
            $"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/submit");
        AllowAnonymous();
        Description(b => b
                .Accepts<SubmitOrderRequest>("application/json")
                .Produces<EmptyResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Submit Order";
            s.Description =  "Submit Order For Selected Styles Configuration";
            s.ExampleRequest = new SubmitOrderRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                SelectedStylesIds = new []{""}
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(SubmitOrderRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        var owner = await _tourStyleConfigurationRepository.GetOwnerAsync(new ConfigurationOwnerIdentity(configuration.OwnerId));
        
        var roomTypes = await _getRoomTypesQuery.GetTourConfigurationRoomTypes(language);

      
       
        
        foreach (var style in configuration.InteriorStyleConfiguration.Styles)
        {
            var defaultProducts = await _styleDefaultProductsProvider.
                GetDefaultProducts(style.BaseStyle.ToString());

            foreach (var defaultProductRoomType in defaultProducts.Select(x=>x.RoomType))
            {
                if (style.RoomsConfigurations.Any(x => string.Equals(x.Type,defaultProductRoomType,StringComparison.OrdinalIgnoreCase)) == false)
                {
                    var roomType = roomTypes.SingleOrDefault(x=>
                        string.Equals(x.RoomType,defaultProductRoomType, StringComparison.OrdinalIgnoreCase));

                    if (roomType == null)
                    {
                        ThrowError($"Configuration not found for roomType: {defaultProductRoomType}");
                    }

                    style.AddRoom(RoomItem.New(roomType.DisplayName, roomType.RoomType, RoomStyle.New(style.BaseStyle), 
                        defaultProducts.Map(roomType.RoomType), isDefinedByUser: false));
                }
            }
        }
        
        configuration.SubmitOrder();
        
        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);

        await SendEmail(c, configuration, owner);
        
        await SendAsync(new EmptyResponse(), cancellation: c);
    }
    
    private async Task SendEmail(CancellationToken c, TourStyleConfiguration styleConfiguration, ConfigurationOwner owner)
    {
        try
        {
            var sendEmailNotificationCommand = new SendEmailNotificationCommand
            {
                ContentTemplate = new ContentTemplate 
                {
                    TemplateUrl = "https://3dnotificationstaging.blob.core.windows.net/email-templates/email-template-order.html",
                    Parameters = new Dictionary<string, string> {
                        {"CODE",styleConfiguration.Code},
                        {"INVESTMENT",styleConfiguration.InvestmentName},
                        {"DEVELOPER",owner.Name},
                        {"EMAIL",owner.Email},
                        {"CONFIGURATION_LINK",$"https://{InteriorDesignerContextProvider.GetHostName(styleConfiguration.InteriorDesignerCode,Env.EnvironmentName)}.3destate.pl"}
                    }
                },
                Recipients = owner.Email+";configurator-orders@3destate.pl",
                SourceServiceCode = "TourConfiguration",
                NotificationId = Guid.NewGuid(),
                SenderEmail = "system@3destate.pl",
                SenderName = "3D Estate Configurator",
                Title = $"Zamówienie {styleConfiguration.Code} zostało wygenerowane.",
            };
            await _emailNotificationService.SendEmailAsync(sendEmailNotificationCommand, c);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}