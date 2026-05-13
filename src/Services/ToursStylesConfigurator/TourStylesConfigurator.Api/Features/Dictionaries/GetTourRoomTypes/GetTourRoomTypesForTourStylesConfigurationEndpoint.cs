using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetTourRoomTypes;

public class Endpoint : Endpoint<GetTourRoomTypesRequest, GetTourRoomTypesResponse>
{
    private readonly IGetRoomTypesQuery _getRoomTypesQuery;


    public Endpoint(IGetRoomTypesQuery getRoomTypesQuery)
    {
        _getRoomTypesQuery = getRoomTypesQuery;
    }

    public override void Configure()
    {
        Get("/tourroomtypes");
        AllowAnonymous();
        Description(b => b
                .Produces<GetTourRoomTypesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Get Tour Room Types";
            s.Description = "Get Tour Room Types";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetTourRoomTypesRequest request, CancellationToken c)
    {

        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var roomTypes = await _getRoomTypesQuery.GetTourRoomTypes(language);
        await SendAsync(new GetTourRoomTypesResponse
        {
            RoomTypes = roomTypes
        }, cancellation: c);
    }
    

}