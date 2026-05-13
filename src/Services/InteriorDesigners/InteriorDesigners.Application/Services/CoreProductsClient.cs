namespace InteriorDesigners.Application.Services;

public interface ICoreProductsServiceClient
{
    Task<List<CoreProductModel>> GetProducts();
}

public class GetProductsResponse
{
    public List<CoreProductModel> Products { get; set; }
}