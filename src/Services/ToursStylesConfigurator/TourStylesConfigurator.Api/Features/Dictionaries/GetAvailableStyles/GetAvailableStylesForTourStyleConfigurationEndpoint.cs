using TourStylesConfigurator.Api.Features.StyleConfigurations;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetAvailableStyles;

public class Endpoint : Endpoint<GetAvailableStylesRequest, GetAvailableStylesResponse>
{
    private readonly IGetTourStylesQuery _getTourStylesQuery;


    public Endpoint(IGetTourStylesQuery getTourStylesQuery)
    {
        _getTourStylesQuery = getTourStylesQuery;
    }

    public override void Configure()
    {
        Get("/tourstyles");
        AllowAnonymous();
        Description(b => b
                .Produces<GetAvailableStylesResponse>(200, "application/json")
                .Accepts<GetAvailableStylesRequest>("application/json"),
        true);
        Summary(s =>
        {
            s.Summary = "Get Available Tour Styles Configuration";
            s.Description = "Get Available Tour Styles Configuration";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetAvailableStylesRequest request, CancellationToken c)
    {
        if (request.Contexts == null || request.Contexts.Count == 0)
        {
            request.Contexts = new List<StyleContext> {StyleContext.Configurator};
        }
        
        var result = await _getTourStylesQuery.GetTourStyles("Livingroom");

        if(request.Contexts.Contains(StyleContext.WithoutContext))
        {
            await SendAsync(new GetAvailableStylesResponse
            {
                TourStyles = result.ToList()
            }, cancellation: c);
        }
        else
        {
            await SendAsync(new GetAvailableStylesResponse
            {
                TourStyles = result.Where(x=>x.Contexts.Any(y=>request.Contexts.Contains(y))).ToList()
            }, cancellation: c);   
        }
    }
}