using Microsoft.AspNetCore.WebUtilities;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;
using TourStylesConfigurator.Api.Infrastructure.Storage;

namespace TourStylesConfigurator.Api.Features.StyleConfigurations.Services;

public interface IGetInteriorDesignerProducts
{
    public Task<List<InteriorDesignerProductDto>> GetProducts(List<string> ids, string interiorDesignerCode, string language);
    public Task<List<InteriorDesignerProductDto>> GetProducts(string interiorDesignerCode, string language);
}


public class GetInteriorDesignerProducts : IGetInteriorDesignerProducts
{
    private readonly IInteriorDesignerServiceClient _interiorDesignerService;


    public GetInteriorDesignerProducts(IInteriorDesignerServiceClient interiorDesignerService)
    {
        _interiorDesignerService = interiorDesignerService;
    }

    public async Task<List<InteriorDesignerProductDto>> GetProducts(List<string> ids, string interiorDesignerCode, string language)
    {
        
        var filters = new List<KeyValuePair<string, string>>
        {
            KeyValuePair.Create("InteriorDesigner", interiorDesignerCode)!,
        };

        foreach (var id in ids ?? new List<string>())
        {
            filters.Add(KeyValuePair.Create<string, string>("Ids",id));
        }

        var response =  await _interiorDesignerService.GetProductsAsync(filters, language);
        
        return  response.Products.SelectMany(x=>x.Products).ToList();
        
    }
    
    public async Task<List<InteriorDesignerProductDto>> GetProducts(string interiorDesignerCode, string language)
    {
        var filters = new List<KeyValuePair<string, string>>
        {
            KeyValuePair.Create("InteriorDesigner", interiorDesignerCode)!,
        };
        
        var response =  await _interiorDesignerService.GetProductsAsync(filters, language);

        return  response.Products.SelectMany(x=>x.Products).ToList();
    }
}