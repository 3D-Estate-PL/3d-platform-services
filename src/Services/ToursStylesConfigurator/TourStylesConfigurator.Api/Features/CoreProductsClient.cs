using TourStylesConfigurator.Api.Features.StyleConfigurations.Services;
using TourStylesConfigurator.Api.Infrastructure.InteriorsDesigners.Products;

namespace TourStylesConfigurator.Api.Features;

public interface ICoreProductsServiceClient
{
    Task<List<StyleDefaultProducts>> GetDefaultStylesProducts();
}


public class CoreProductsServiceClient : ICoreProductsServiceClient
{
    private readonly HttpClient _httpClient;


    public CoreProductsServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StyleDefaultProducts>> GetDefaultStylesProducts()
    {
        var result = await _httpClient.GetAsync($"api/Styles/DefaultProducts");
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<GetDefaultStylesProducts>();
        return response.Items;
    }
}