using System.Net.Http.Json;
using InteriorDesigners.Application.Services;

namespace InteriorDesigners.Infrastructure;

public class CoreProductsServiceClient : ICoreProductsServiceClient
{
    private readonly HttpClient _httpClient;

    public CoreProductsServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<CoreProductModel>> GetProducts()
    {
        var result = await _httpClient.GetAsync($"api/products");
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<GetProductsResponse>();
        return response?.Products ?? new List<CoreProductModel>();
    }
}