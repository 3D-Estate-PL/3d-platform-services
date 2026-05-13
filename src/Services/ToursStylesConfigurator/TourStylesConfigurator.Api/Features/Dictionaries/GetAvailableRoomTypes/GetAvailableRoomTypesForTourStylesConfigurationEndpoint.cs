using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableRoomTypes;

public class Endpoint : Endpoint<GetAvailableRoomTypesRequest, GetAvailableRoomTypesResponse>
{
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;


    public Endpoint(IGetRoomTypesQuery getRoomTypesQuery)
    {
        _getRoomTypesQuery = getRoomTypesQuery;
    }

    public override void Configure()
    {
        Get("/roomtypes");
        AllowAnonymous();
        Description(b => b
                .Produces<GetAvailableRoomTypesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Get Available Room Types For Configuration";
            s.Description = "Get Available Room Types For Configuration";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetAvailableRoomTypesRequest request, CancellationToken c)
    {

        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var roomTypes = await _getRoomTypesQuery.GetTourConfigurationRoomTypes(language);
        roomTypes = roomTypes.Where(x =>string.Equals( x.Category,"Interior",StringComparison.OrdinalIgnoreCase)).ToList();
        await SendAsync(new GetAvailableRoomTypesResponse
        {
            RoomTypes = roomTypes
        }, cancellation: c);
    }
    

}