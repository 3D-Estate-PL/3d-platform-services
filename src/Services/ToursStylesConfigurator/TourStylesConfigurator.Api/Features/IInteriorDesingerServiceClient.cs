using Microsoft.AspNetCore.WebUtilities;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetails;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProductDetailsV2;
using TourStylesConfigurator.Api.Features.InteriorsDesigners.Products.GetProducts;
using TourStylesConfigurator.Api.Infrastructure.InteriorsDesigners.Products;

namespace TourStylesConfigurator.Api.Features;


public interface IInteriorDesignerServiceClient
{
    Task<GetProductDetailResponse> GetAsync(string interiorDesignerCode, string productId, string language);
    Task<GetProductDetailResponse2> GetProductDetailWithInteriorDesingersAsync(string requestId, string language);
    Task<GetProductsResponse> GetProductsAsync(List<KeyValuePair<string, string>> filters, string language);
}

public class InteriorDesignerServiceClient : IInteriorDesignerServiceClient
{
    private readonly HttpClient _httpClient;

    public InteriorDesignerServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetProductDetailResponse> GetAsync(string interiorDesignerCode, string productId, string language)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.Add("accept-language",language);
        var result = await _httpClient.GetAsync($"api/InteriorsDesigners/{interiorDesignerCode}/products/{productId}");
        var response = await result.Content.ReadFromJsonAsync<GetProductDetailResponse>();
        return response;
    }

    public async Task<GetProductDetailResponse2> GetProductDetailWithInteriorDesingersAsync(string productId, string language)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.Add("Accept-Language",language);
        var result = await _httpClient.GetAsync($"api/products/{productId}");
        var response = await result.Content.ReadFromJsonAsync<GetProductDetailResponse2>();
        return response;
    }

    public async Task<GetProductsResponse> GetProductsAsync(List<KeyValuePair<string, string>> filters, string language)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.Add("Accept-Language",language);
        var result = await _httpClient.GetAsync(QueryHelpers.AddQueryString("api/Products", filters));
        var response = await result.Content.ReadFromJsonAsync<GetProductsResponse>();
        return response;
    }
}