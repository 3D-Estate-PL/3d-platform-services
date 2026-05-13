using BuildingBlocks.Domain.DDD;

namespace InteriorDesigners.Domain.InteriorDesigner;

public class ProductIdentity : DocumentIdentity<ProductAggregate>
{
    public string InteriorDesignerCode { get; }

    public ProductIdentity(string id, string interiorDesignerCode) : base(id)
    {
        InteriorDesignerCode = interiorDesignerCode;
    }

    public override string DocumentId => $"{InteriorDesignerCode}|{Id}";
    public override string PartitionKey => "InteriorDesignerProducts";
}