using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetCategories;

public class GetProductCategoriesEndpoint : Endpoint<GetProductsCategoriesRequest, GetProductsCategoriesResponse>
{
    private readonly IGetRoomTypeCategoriesQuery _getRoomTypeCategoriesQuery;
    private readonly ILogger<GetProductCategoriesEndpoint> _logger;


    public GetProductCategoriesEndpoint(IGetRoomTypeCategoriesQuery getRoomTypeCategoriesQuery, ILogger<GetProductCategoriesEndpoint> logger)
    {
        _getRoomTypeCategoriesQuery = getRoomTypeCategoriesQuery;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/productscategories");
        AllowAnonymous();
        Description(b => b
                .Produces<GetProductsCategoriesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Get Products Categories";
            s.Description = "Get Products Categories";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetProductsCategoriesRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        
        var roomTypeCategories = await _getRoomTypeCategoriesQuery.GetRoomTypeCategories(language);

        await SendAsync(new GetProductsCategoriesResponse
        {
            ProductsCategories = roomTypeCategories.ToList()
        }, cancellation: c);
    }

   
}