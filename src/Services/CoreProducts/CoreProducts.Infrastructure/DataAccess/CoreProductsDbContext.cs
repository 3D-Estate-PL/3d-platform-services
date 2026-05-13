using BuildingBlocks.Infrastructure.DataAccess.Cosmos;
using CoreProducts.Domain.Products;
using CoreProducts.Infrastructure.DataAccess.Products;

namespace CoreProducts.Infrastructure.DataAccess;

public class CoreProductsDbContext : CosmosContext
{
    public CoreProductsDbContext(ProductDatabaseConfiguration cosmosDbClient) 
        : base(cosmosDbClient)
    {
        Map<Product>("coreProducts-products");
        Map<Domain.DefaultProducts.StyleDefaultProducts>("coreProducts-styleDefaultProducts");
    }
}