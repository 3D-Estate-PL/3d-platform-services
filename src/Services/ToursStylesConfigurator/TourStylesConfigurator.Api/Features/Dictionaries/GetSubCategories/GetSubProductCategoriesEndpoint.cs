using BuildingBlocks.Abstractions.Excel;
using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

namespace TourStylesConfigurator.Api.Features.Dictionaries.GetSubCategories;

public class GetProductSubCategoriesEndpoint : Endpoint<GetProductsCategoriesRequest, GetProductsCategoriesResponse>
{
    private readonly IGetProductSubCategoriesQuery _getProductSubCategories;
    private readonly IExcelDictionaryProvider _excelDictionaryProvider;
    private readonly ILogger<GetProductSubCategoriesEndpoint> _logger;


    public GetProductSubCategoriesEndpoint(IGetProductSubCategoriesQuery getProductSubCategories,
        IExcelDictionaryProvider excelDictionaryProvider,
        ILogger<GetProductSubCategoriesEndpoint> logger)
    {
        _getProductSubCategories = getProductSubCategories;
        _excelDictionaryProvider = excelDictionaryProvider;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/productssubcategories");
        AllowAnonymous();
        Description(b => b
                .Produces<GetProductsCategoriesResponse>(200, "application/json"),
            clearDefaults: true);
        Summary(s => {
            s.Summary = "Get Products SubCategories";
            s.Description = "Get Products SubCategories";
            s.Responses[200] = "Empty";
        });
    }

    public override async Task HandleAsync(GetProductsCategoriesRequest request, CancellationToken c)
    {
        var userLangs = HttpContext.Request.Headers["accept-language"].ToString();
        var language = userLangs.Split(',').FirstOrDefault();
        var roomTypeCategories = await _getProductSubCategories.GetAll(language);

        await SendAsync(new GetProductsCategoriesResponse
        {
            ProductsSubCategories = roomTypeCategories
        }, cancellation: c);
    }
}