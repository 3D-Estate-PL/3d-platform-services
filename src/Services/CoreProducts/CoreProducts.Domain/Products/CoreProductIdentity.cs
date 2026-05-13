using BuildingBlocks.Domain.DDD;

namespace CoreProducts.Domain.Products;

public class CoreProductIdentity : DocumentIdentity<Product>
{
    public CoreProductIdentity(string id) : base(id)
    {
    }
}