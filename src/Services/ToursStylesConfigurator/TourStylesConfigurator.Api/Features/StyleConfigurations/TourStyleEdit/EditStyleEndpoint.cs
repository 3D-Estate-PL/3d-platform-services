using TourStylesConfigurator.Api.Features.StyleConfigurations.Model;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels;
using TourStylesConfigurator.Api.Infrastructure.StyleConfigurations.DomainModels.Styles;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.TourStyleEdit;

public class Endpoint : Endpoint<EditTourStyleRequest, EditTourStyleConfigurationResponse>
{
    private readonly ITourStyleConfigurationRepository _tourStyleConfigurationRepository;
    private readonly IStyleDefaultProductsProvider _styleDefaultProductsProvider;
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;


    public Endpoint(ITourStyleConfigurationRepository tourStyleConfigurationRepository, IStyleDefaultProductsProvider styleDefaultProductsProvider, IGetRoomTypesQuery getRoomTypesQuery)
    {
        _tourStyleConfigurationRepository = tourStyleConfigurationRepository;
        _styleDefaultProductsProvider = styleDefaultProductsProvider;
        _getRoomTypesQuery = getRoomTypesQuery;
    }

    public override void Configure()
    {
        Put($"/{Paths.TourStyleConfigurations}/{{ConfigurationId}}/{Paths.Place}/{Paths.Styles}/{{ConfigurationStyleId}}");
        AllowAnonymous();
        Description(b => b
                .Accepts<EditTourStyleRequest>("application/json")
                .Produces<EditTourStyleConfigurationResponse>(200, "application/json"),
            true);
        Summary(s =>
        {
            s.Summary = "Copy tour style";
            s.Description = "Copy tour style";
            s.Description = "Copy tour style";
            s.ExampleRequest = new EditTourStyleRequest
            {
                ConfigurationId = "61e556cf-eb0a-4aa5-8d9d-02937e93e9c9",
                CustomName = $"Nowoczesne mieszkanie dla Par kopia",
                Place = PlaceType.Interior,
                ConfigurationStyleId = "InsertGuid"
            };
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(EditTourStyleRequest request, CancellationToken c)
    {
        var configuration = await _tourStyleConfigurationRepository.GetConfigurationAsync(request.ConfigurationId);
        
        if (configuration.Status != StyleConfigurationStatus.Draft)
        {
            ThrowError("Can not change style. Configuration is marked as readonly.");
        }
        
        var place = configuration.GetPlace(request.Place);
        var tourStyleConfiguration = place.Styles
            .SingleOrDefault(x => x.Id == request.ConfigurationStyleId);

        if (tourStyleConfiguration == null)
        {
            ThrowError("Tour Style Configuration Not Found.");
        }

        tourStyleConfiguration.CustomName = request.CustomName;

        await _tourStyleConfigurationRepository.UpsertConfigurationAsync(configuration);


        var response = new EditTourStyleConfigurationResponse
        {
            Id  = configuration.Id,
            CustomName = tourStyleConfiguration.CustomName,
            Code = tourStyleConfiguration.Code,
            RoomsConfigurations = tourStyleConfiguration.RoomsConfigurations.Select(x=> new RoomConfigurationDto
            {
                Id = x.Id,
                CustomName = x.CustomName,
                RoomType = x.Type,
                IsRequired = x.IsRequired,
                Style = new RoomStyleBaseDto
                {
                    BaseStyle = x.SelectedStyle.BaseStyle,
                    IsCustom = x.SelectedStyle.IsCustom
                }
                
            }).ToList()
        };
        
         
        await SendAsync(response, cancellation: c);
    }
}