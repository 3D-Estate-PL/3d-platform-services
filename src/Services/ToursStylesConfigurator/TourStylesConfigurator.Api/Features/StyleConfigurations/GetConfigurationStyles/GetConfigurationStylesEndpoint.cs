using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.GetConfigurationStyles;

public class Endpoint : Endpoint<GetConfigurationStylesRequest, GetConfigurationStylesResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
    }

    public override void Configure()
    {
        Get($"/{Paths.TourStyleConfigurations}/{{ConfigurationCode}}/styles/{{StyleCode}}");
        AllowAnonymous();
        Description(b => b
                .Produces<GetConfigurationStylesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.ExampleRequest = new GetConfigurationStylesRequest()
            {
               ConfigurationCode = "Q5XB2",
               StyleCode = "1_Nowoczesne_Mieszkanie"
            };
            s.Summary = "Get Configuration Styles";
            s.Description = "Get Configuration Styles";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetConfigurationStylesRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationByCodeAsync(request.ConfigurationCode);


        
        var style = configuration.InteriorStyleConfiguration.Styles.Single(x => x.Code == request.StyleCode);
        
        var response = new GetConfigurationStylesResponse
        {
            Code = $"{request.ConfigurationCode}_{request.StyleCode}",
            BaseStyle = style.BaseStyle,
            InteriorDesignerCode = configuration.InteriorDesignerCode,
            Rooms = new List<RoomItemDto>()
        };
        
        if (configuration.Status == StyleConfigurationStatus.Draft)
        {
            await SendAsync(new GetConfigurationStylesResponse(), cancellation: c);
        }
        
        var index = 0;
        foreach (var room in style.RoomsConfigurations)
        {
            var roomItemDto = new RoomItemDto
            {
                Room = room.Type.ToPascalCase(),
                RoomIndex = index++
            };
            response.Rooms.Add(roomItemDto);

            foreach (var product in room.GetProducts())
            {
                roomItemDto.Elements.Add(new ProductItemDto
                {
                    Id = product.ProductId,
                    CategoryName = $"{roomItemDto.Room}.{product.CategoryName}",
                });
            }
        }

        await SendAsync(response, cancellation: c);
    }
}